﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html;
using AngleSharp.Io;
using AngleSharp.Js;
using AngleSharp.Text;
using Markdig;
using Markdig.Syntax;
using SharpGen.Doc;
using SharpGen.Extension.MicrosoftDocs.XmlDoc;
using SharpGen.Extension.MicrosoftDocs.XmlDoc.YamlHeader;

[assembly: InternalsVisibleTo("SharpGen.UnitTests")]

namespace SharpGen.Extension.MicrosoftDocs
{
    public sealed class DocsProvider : IDocProvider
    {
        private const string GitHubPartDomain = "//github.com/";
        private const string GitHubPartBlob = "/blob/";
        private const string MarkdownMessageLogCode = "MD0001";
        private static readonly Dictionary<Regex, string> CommonReplaceRuleMap = new Dictionary<Regex, string>();

        private static readonly Regex ParamAttributesRegEx =
            new Regex(@"\s*(.+?)\s*\[\s*(.+?)\s*\]\s*", RegexOptions.Compiled);

        private static readonly JsonDocumentOptions JsonDocumentOptions = new JsonDocumentOptions
        {
            CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true
        };

        private static readonly HttpClient Client = new HttpClient();

        private static readonly LoaderOptions LoaderOptions = new LoaderOptions {IsNavigationDisabled = true};

        private static readonly IConfiguration AngleSharpRemoteConfig =
            Configuration.Default.WithDefaultLoader().WithJs();

        private static readonly IConfiguration AngleSharpMarkdownConfig =
            Configuration.Default.WithDefaultLoader(LoaderOptions);

        private static readonly MinifyMarkupFormatter MinifyMarkupFormatter = new MinifyMarkupFormatter
        {
            ShouldKeepAttributeQuotes = true,
            ShouldKeepImpliedEndTag = true,
            PreservedTags = new[]
            {
                "c", "code", TagNames.Pre, TagNames.Textarea
            }
        };

        static DocsProvider()
        {
            ReplaceName("W::", @"::");
            ReplaceName("([a-z0-9])A::", @"$1::");
            ReplaceName("W$", @"");
            ReplaceName("^_+", @"");
        }

        private static MarkdownPipeline CreateMarkdownPipeline(IDocumentationContext context)
        {
            var markdownContext = new MarkdownContext(logInfo: LogInfo, logSuggestion: LogSuggestion,
                                                      logWarning: LogWarning, logError: LogError);
            var builder = new MarkdownPipelineBuilder().UseDocfxExtensions(markdownContext);
            builder.Extensions.Insert(0, new YamlHeaderExtension());
            return builder.Build();

            void LogInfo(string code, string message, MarkdownObject origin, int? line)
            {
                context.Logger.Message(message);
            }

            void LogSuggestion(string code, string message, MarkdownObject origin, int? line)
            {
                context.Logger.Message(message);
            }

            void LogWarning(string code, string message, MarkdownObject origin, int? line)
            {
                context.Logger.Warning(MarkdownMessageLogCode, message);
            }

            void LogError(string code, string message, MarkdownObject origin, int? line)
            {
                context.Logger.Error(MarkdownMessageLogCode, message);
            }
        }

        internal sealed class SearchResult
        {
            public SearchResult(IReadOnlyList<string> names) => Names = names ?? throw new ArgumentNullException(nameof(names));

            public IReadOnlyList<string> Names { get; }
            public string PublishedUrl { get; set; }
        }

        public async Task<IDocItem> FindDocumentationAsync(string name, IDocumentationContext context)
        {
            var searchResult = await SearchDocumentation(name);

            if (searchResult == null)
                return null;

            var sourceUrl = await GetSourceUrlFromPublishedDocumentation(searchResult.PublishedUrl);

            if (string.IsNullOrEmpty(sourceUrl))
                return null;

            var docItem = await ParseDocumentationFromSourceUrl(sourceUrl, context);

            if (docItem == null)
                return null;

            foreach (var nameItem in searchResult.Names)
                docItem.Names.Add(nameItem);

            var docUri = new Uri(searchResult.PublishedUrl, UriKind.Absolute);
            docItem.ShortId = Path.GetFileNameWithoutExtension(docUri.Segments.Last());

            return docItem;
        }

        internal static async Task<IDocItem> ParseDocumentationFromSourceUrl(string sourceUrl, IDocumentationContext context)
        {
            var doc = await GetXmlDocFromDocumentationSourceUrl(sourceUrl, context);

            if (string.IsNullOrEmpty(doc))
                return null;

            var docItem = context.CreateItem();

            await ParseDocumentation(docItem, doc, context);

            return docItem;
        }

        internal async Task<SearchResult> SearchDocumentation(string name)
        {
            if (name.Length < 3)
                return null;

            var result = new SearchResult(TransformRequestedName(name));

            result.PublishedUrl = await FindBestSearchResult(result.Names);

            return string.IsNullOrEmpty(result.PublishedUrl) ? null : result;
        }

        private static void ReplaceName(string fromNameRegex, string toName)
        {
            CommonReplaceRuleMap.Add(new Regex(fromNameRegex, RegexOptions.Compiled), toName);
        }

        private static async Task<string> FindBestSearchResult(IReadOnlyList<string> names)
        {
            var count = names.Count;
            var documents = new JsonDocument[count];

            try
            {
                var variants = new ImmutableArray<JsonElement>[count];

                for (var i = 0; i < count; i++)
                {
                    var name = names[i];
                    var searchTuple = await PerformDocsSearch(name);
                    if (!searchTuple.HasValue)
                        continue;

                    (documents[i], variants[i]) = searchTuple.Value;

                    var nameRegex = new Regex(
                        $@"\b{Regex.Escape(name)}\b",
                        RegexOptions.CultureInvariant | RegexOptions.IgnoreCase
                    );

                    var result = TrySearchVariant(variants, i, nameRegex);

                    if (result.HasValue) return ProcessBestMatchingSearchResult(result.Value);
                }

                var jsonResultElement = variants.FirstOrDefault(x => !x.IsDefaultOrEmpty);

                if (jsonResultElement.IsDefaultOrEmpty)
                    return null;

                return ProcessBestMatchingSearchResult(jsonResultElement.First());
            }
            finally
            {
                for (var i = 0; i < count; i++) documents[i]?.Dispose();
            }
        }

        private static JsonElement? TrySearchVariant(ImmutableArray<JsonElement>[] variants, int i, Regex name)
        {
            ref var variant = ref variants[i];
            var length = variant.Length;

            for (var index = 0; index < length; index++)
            {
                var jsonElement = variant[index];
                if (IsSearchResultExactMatch(name, jsonElement))
                    return jsonElement;
            }

            return null;
        }

        private static IReadOnlyList<string> TransformRequestedName(string name)
        {
            var names = new List<string>(2) {name};

            // Regex replacer
            foreach (var keyValue in CommonReplaceRuleMap)
            {
                if (!keyValue.Key.Match(name).Success) continue;

                name = keyValue.Key.Replace(name, keyValue.Value);
                names.Add(name);
                break;
            }

            // Handle name with ends A or W
            if (name.EndsWith("A") || name.EndsWith("W"))
            {
                var previousChar = name[name.Length - 2];

                if (!char.IsUpper(previousChar))
                {
                    name = name.Substring(0, name.Length - 1);
                    names.Add(name);
                }
            }

            var nameSet = new HashSet<string>();
            var result = new List<string>(names.Count);

            // ReSharper issue: non-equivalent transformation relying on undocumented Distinct behavior
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var item in names)
                if (nameSet.Add(item))
                    result.Add(item);

            return result;
        }

        private static async Task ParseDocumentation(IDocItem item, string documentationToParse,
                                                     IDocumentationContext context)
        {
            if (string.IsNullOrEmpty(documentationToParse))
                return;

            var browsingContext = BrowsingContext.New(AngleSharpMarkdownConfig);
            var document = await browsingContext.OpenAsync(req => req.Content(documentationToParse));
            await document.WaitUntilAvailable();

            foreach (var element in document.QuerySelectorAll("img"))
                element.Remove();
            foreach (var element in document.QuerySelectorAll("a, em"))
                ReplaceTagName(document, "i", element, false);
            foreach (var element in document.QuerySelectorAll("br"))
                ReplaceBrWithPara(element, document);
            foreach (var element in document.QuerySelectorAll("div"))
                ReplaceTagName(document, "para", element, false);
            foreach (var element in document.QuerySelectorAll("strong"))
                ReplaceTagName(document, "b", element, false);

            foreach (var element in document.QuerySelectorAll("para").Where(x => !x.HasChildNodes))
                element.Remove();

            var h1 = document.QuerySelector("h1");

            static string HeadingKeySelector(IElement element) => element.InnerHtml.Trim();

            var blocks = SplitAt(h1.NextElementSibling, "h1, h2")
               .ToDictionary(tuple => HeadingKeySelector(tuple.Heading), tuple => tuple.Children);

            static string ElementsToString(ImmutableArray<IElement> immutableArray)
            {
                if (immutableArray.Length == 1 && immutableArray[0].LocalName == "para")
                    return immutableArray[0].InnerHtml.Trim();

                return string.Concat(immutableArray.Select(x => x.ToHtml(MinifyMarkupFormatter)));
            }

            if (blocks.TryGetValue("-description", out var description) && description.Length != 0)
                item.Summary = ElementsToString(description);

            if (blocks.TryGetValue("-returns", out var returns) && returns.Length != 0)
                item.Return = ElementsToString(returns);

            if (blocks.TryGetValue("-remarks", out var remarks) && remarks.Length != 0)
                item.Remarks = ElementsToString(remarks);

            if (blocks.TryGetValue("-parameters", out var parameters) && parameters.Length != 0)
            {
                foreach (var kv in ExtractSubItems(parameters))
                {
                    var (param, value) = (kv.Key, kv.Value);

                    if (param.StartsWith("-param", StringComparison.InvariantCultureIgnoreCase))
                        param = param.ReplaceFirst("-param", string.Empty).TrimStart();

                    ImmutableArray<string> attributes = default;

                    var match = ParamAttributesRegEx.Match(param);
                    if (match.Success)
                    {
                        param = match.Groups[1].Value.Trim();
                        var attributesString = match.Groups[2].Value;

                        if (!string.IsNullOrEmpty(attributesString))
                            attributes = attributesString.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                                                         .Select(x => x.Trim())
                                                         .ToImmutableArray();
                    }

                    var subItem = context.CreateSubItem();
                    subItem.Term = param;
                    subItem.Description = ElementsToString(value);

                    if (!attributes.IsDefaultOrEmpty)
                        foreach (var attribute in attributes)
                            subItem.Attributes.Add(attribute);

                    item.Items.Add(subItem);
                }
            }

            if (blocks.TryGetValue("-enum-fields", out var enumFields) && enumFields.Length != 0)
            {
                ProcessSubItems(enumFields);
            }

            if (blocks.TryGetValue("-struct-fields", out var structFields) && structFields.Length != 0)
            {
                ProcessSubItems(structFields);
            }

            Dictionary<string, ImmutableArray<IElement>> ExtractSubItems(ImmutableArray<IElement> elements)
            {
                var wrapper = WrapAll(elements, document.CreateElement("div"));
                return SplitAt(wrapper.QuerySelector("h3"), "h1, h2, h3")
                   .ToDictionary(tuple => HeadingKeySelector(tuple.Heading), tuple => tuple.Children);
            }

            void ProcessSubItems(ImmutableArray<IElement> fields)
            {
                foreach (var kv in ExtractSubItems(fields))
                {
                    var (param, value) = (kv.Key, kv.Value);

                    if (param.StartsWith("-field", StringComparison.InvariantCultureIgnoreCase))
                        param = param.ReplaceFirst("-field", string.Empty).TrimStart();

                    var subItem = context.CreateSubItem();
                    subItem.Term = param;
                    subItem.Description = ElementsToString(value);

                    item.Items.Add(subItem);
                }
            }
        }

        private static void ReplaceTagName(IDocument document, string elementName, IElement element, bool moveAttributes)
        {
            var replacement = document.CreateElement(elementName);
            replacement.InnerHtml = element.InnerHtml;
            if (moveAttributes)
                foreach (var elementAttribute in element.Attributes)
                    replacement.SetAttribute(elementAttribute.LocalName, elementAttribute.Value);
            element.ReplaceWith(replacement);
        }

        private static void ReplaceBrWithPara(INode element, IDocument document)
        {
            var parent = element.ParentElement;
            if (parent == null) return;

            var contents = parent.ChildNodes.ToArray();
            var set = new List<INode>();
            foreach (var cur in contents)
                if (BrPredicate(cur))
                {
                    WrapSet();
                    cur.RemoveFromParent();
                }
                else
                {
                    set.Add(cur);
                }

            WrapSet();

            void WrapSet()
            {
                if (set.Count == 0)
                    return;

                WrapAll(set, document.CreateElement("para"));
                set.Clear();
            }

            static bool BrPredicate(INode cur) =>
                cur.NodeType == NodeType.Element &&
                string.Equals(cur.NodeName, "br", StringComparison.OrdinalIgnoreCase);
        }

        private static IEnumerable<(IElement Heading, ImmutableArray<IElement> Children)> SplitAt(
            IElement element, string selector)
        {
            var heading = element;
            var builder = ImmutableArray.CreateBuilder<IElement>();

            for (element = element.NextElementSibling; element != null; element = element.NextElementSibling)
                if (element.Matches(selector))
                {
                    yield return (heading, builder.ToImmutable());
                    builder.Clear();
                    heading = element;
                }
                else
                {
                    builder.Add(element);
                }

            yield return (heading, builder.ToImmutable());
        }

        private static IElement WrapAll(IReadOnlyList<INode> nodes, IElement wrapper)
        {
            Debug.Assert(nodes.Count != 0);

            // Cache the current parent and previous sibling of the first node.
            var parent = nodes[0].ParentElement;
            var previousSibling = nodes[0].PreviousSibling;

            // Place each node in wrapper.
            foreach (var child in nodes)
                wrapper.AppendChild(child);

            // Place the wrapper just after the cached previousSibling,
            // or if that is null, just before the first child.
            var nextSibling = previousSibling != null ? previousSibling.NextSibling : parent.FirstChild;
            parent.InsertBefore(wrapper, nextSibling);

            return wrapper;
        }

        private static async Task<string> GetSourceUrlFromPublishedDocumentation(string docsUrl)
        {
            var gitUrl = await RetrieveGitHubUrl(docsUrl);

            if (string.IsNullOrEmpty(gitUrl))
                return null;

            if (!gitUrl.Contains(GitHubPartDomain) || !gitUrl.Contains(GitHubPartBlob))
                return null;

            gitUrl = gitUrl.ReplaceFirst(GitHubPartDomain, "//raw.githubusercontent.com/").ReplaceFirst(GitHubPartBlob, "/");

            return gitUrl;
        }

        private static async Task<string> GetXmlDocFromDocumentationSourceUrl(string gitUrl, IDocumentationContext context)
        {
            var markdownString = await GetFromUrl(gitUrl);
            var markdownPipeline = CreateMarkdownPipeline(context);
            var markdownDocument = Markdown.Parse(markdownString, markdownPipeline);

            var writer = new StringWriter(new StringBuilder(1024), CultureInfo.InvariantCulture);
            var markdownRenderer = new XmlDocRenderer(writer);
            markdownPipeline.Setup(markdownRenderer);
            markdownRenderer.Render(markdownDocument);
            await writer.FlushAsync();

            return writer.ToString();
        }

        private static async Task<string> RetrieveGitHubUrl(string shortId)
        {
            try
            {
                var context = BrowsingContext.New(AngleSharpRemoteConfig);
                var document = await context.OpenAsync(shortId);
                await document.WaitUntilAvailable();
                var gitUrl = document.ExecuteScript("msDocs.data.contentGitUrl") as string;
                return gitUrl;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static async Task<(JsonDocument, ImmutableArray<JsonElement>)?> PerformDocsSearch(string name)
        {
            try
            {
                var searchUrl =
                    $"https://docs.microsoft.com/api/search?search={WebUtility.UrlEncode(name)}&locale=en-us&%24top=10";

                var urlResult = await GetJsonFromUrl(searchUrl, JsonDocumentOptions);

                if (urlResult == null)
                    return default;

                if (!urlResult.RootElement.TryGetProperty("results", out var jsonResultsElement))
                    return default;

                if (jsonResultsElement.ValueKind != JsonValueKind.Array)
                    return default;

                return jsonResultsElement.GetArrayLength() == 0
                           ? default
                           : (urlResult, jsonResultsElement.EnumerateArray().ToImmutableArray());
            }
            catch (Exception)
            {
                return default;
            }
        }

        private static string ProcessBestMatchingSearchResult(JsonElement jsonResultElement)
        {
            if (!jsonResultElement.TryGetProperty("url", out var jsonUrlProperty))
                return string.Empty;

            return jsonUrlProperty.ValueKind == JsonValueKind.String
                       ? jsonUrlProperty.GetString()
                       : string.Empty;
        }

        private static bool IsSearchResultExactMatch(Regex name, JsonElement element)
        {
            if (!element.TryGetProperty("title", out var jsonTitleProperty))
                return false;

            if (jsonTitleProperty.ValueKind != JsonValueKind.String)
                return false;

            var title = jsonTitleProperty.GetString();

            return !string.IsNullOrEmpty(title) && name.Match(title).Success;
        }

        private static async Task<string> GetFromUrl(string url)
        {
            try
            {
                return await Client.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return string.Empty;
            }
        }

        private static async Task<JsonDocument> GetJsonFromUrl(string url, JsonDocumentOptions options = default)
        {
            try
            {
                // Create web request
                using var stream = await Client.GetStreamAsync(url);

                // Get response for http web request
                return await JsonDocument.ParseAsync(stream, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}