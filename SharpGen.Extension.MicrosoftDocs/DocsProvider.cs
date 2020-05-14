using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using AngleSharp.Js;
using AngleSharp.Text;
using Markdig;
using SharpGen.Doc;
using SharpGen.Extension.MicrosoftDocs.XmlDoc;

namespace SharpGen.Extension.MicrosoftDocs
{
    public sealed class DocsProvider : IDocProvider
    {
        private static readonly Dictionary<Regex, string> CommonReplaceRuleMap = new Dictionary<Regex, string>();

        static DocsProvider()
        {
            ReplaceName("W::", @"::");
            ReplaceName("([a-z0-9])A::", @"$1::");
            ReplaceName("W$", @"");
            ReplaceName("^_+", @"");
        }

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

        private static void ReplaceName(string fromNameRegex, string toName)
        {
            CommonReplaceRuleMap.Add(new Regex(fromNameRegex, RegexOptions.Compiled), toName);
        }

        public async Task<IDocItem> FindDocumentationAsync(string name, IDocumentationContext context)
        {
            if (name.Length < 3)
                return null;

            var names = TransformRequestedName(name, out var nameSet);

            var docUrl = await FindBestSearchResult(names);

            if (string.IsNullOrEmpty(docUrl))
                return null;

            var doc = await GetDocumentationFromMsdn(docUrl);

            if (string.IsNullOrEmpty(doc))
                return null;

            var docItem = context.CreateItem();

            foreach (var nameItem in nameSet)
                docItem.Names.Add(nameItem);

            var docUri = new Uri(docUrl, UriKind.Absolute);
            docItem.ShortId = Path.GetFileNameWithoutExtension(docUri.Segments.Last());

            await ParseDocumentation(docItem, doc, context);

            return docItem;
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

                    var result = TrySearchVariant(variants, i, name);

                    if (result.HasValue)
                    {
                        return ProcessBestMatchingSearchResult(result.Value);
                    }
                }

                var jsonResultElement = variants.FirstOrDefault(x => x.Length != 0);

                if (jsonResultElement.IsDefaultOrEmpty)
                    return null;

                return ProcessBestMatchingSearchResult(jsonResultElement.First());
            }
            finally
            {
                for (var i = 0; i < count; i++) documents[i]?.Dispose();
            }
        }

        private static JsonElement? TrySearchVariant(ImmutableArray<JsonElement>[] variants, int i, string name)
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

        private static IReadOnlyList<string> TransformRequestedName(string name, out HashSet<string> nameSet)
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

            nameSet = new HashSet<string>();
            var result = new List<string>(names.Count);

            // ReSharper issue: non-equivalent transformation relying on undocumented Distinct behavior
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var item in names)
                if (nameSet.Add(item))
                    result.Add(item);

            return result;
        }

        private static async Task ParseDocumentation(IDocItem item, string documentationToParse, IDocumentationContext context)
        {
            if (string.IsNullOrEmpty(documentationToParse))
                return;

            var browsingContext = BrowsingContext.New(AngleSharpMarkdownConfig);
            var document = await browsingContext.OpenAsync(req => req.Content(documentationToParse));
            await document.WaitUntilAvailable();

            var h1 = document.QuerySelector("h1");

            static string HeadingKeySelector(IElement element) => element.InnerHtml.Trim();

            var blocks = SplitAt(h1.NextElementSibling, "h1, h2")
               .ToDictionary(tuple => HeadingKeySelector(tuple.Heading), tuple => tuple.Children);

            static string ElementsToString(ImmutableArray<IElement> immutableArray)
            {
                if (immutableArray.Length == 1 && immutableArray[0].LocalName == "para")
                    return immutableArray[0].InnerHtml;

                return string.Concat(immutableArray.Select(x => x.OuterHtml));
            }

            if (blocks.TryGetValue("-description", out var description) && description.Length != 0)
                item.Summary = ElementsToString(description);

            if (blocks.TryGetValue("-returns", out var returns) && returns.Length != 0)
                item.Return = ElementsToString(returns);

            if (blocks.TryGetValue("-remarks", out var remarks) && remarks.Length != 0)
                item.Remarks = ElementsToString(remarks);

            if (blocks.TryGetValue("-parameters", out var parameters) && parameters.Length != 0)
            {
                var wrapper = WrapAll(parameters, document.CreateElement("div"));
                var parametersMap = SplitAt(wrapper.QuerySelector("h3"), "h1, h2, h3")
                   .ToDictionary(tuple => HeadingKeySelector(tuple.Heading), tuple => tuple.Children);

                foreach (var kv in parametersMap)
                {
                    var (param, value) = (kv.Key, kv.Value);

                    if (param.StartsWith("-param", StringComparison.InvariantCultureIgnoreCase))
                        param = param.ReplaceFirst("-param", string.Empty).TrimStart();

                    var subItem = context.CreateSubItem();
                    subItem.Term = param;
                    subItem.Description = ElementsToString(value);

                    item.Items.Add(subItem);
                }
            }
        }

        private static IEnumerable<(IElement Heading, ImmutableArray<IElement> Children)> SplitAt(
            IElement element, string selector)
        {
            var heading = element;
            var builder = ImmutableArray.CreateBuilder<IElement>();

            for (element = element.NextElementSibling; element != null; element = element.NextElementSibling)
            {
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
            }

            yield return (heading, builder.ToImmutable());
        }

        private static IElement WrapAll(ImmutableArray<IElement> nodes, IElement wrapper)
        {
            Debug.Assert(nodes.Length != 0);

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

        private static async Task<string> GetDocumentationFromMsdn(string docsUrl)
        {
            var gitUrl = await RetrieveGitHubUrl(docsUrl);
            if (string.IsNullOrEmpty(gitUrl))
                return null;

            gitUrl = gitUrl.ReplaceFirst("//github.com/", "//raw.githubusercontent.com/").ReplaceFirst("/blob/", "/");

            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var markdownString = await GetFromUrl(gitUrl);
            var markdownDocument = Markdown.Parse(markdownString, pipeline);

            var writer = new StringWriter(new StringBuilder(512), CultureInfo.InvariantCulture);
            var markdownRenderer = new XmlDocRenderer(writer);
            pipeline.Setup(markdownRenderer);
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
                    $"https://docs.microsoft.com/api/search?search={WebUtility.UrlEncode(name)}&locale=en-us&%24top=3";

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

        private static bool IsSearchResultExactMatch(string name, JsonElement element)
        {
            if (!element.TryGetProperty("title", out var jsonTitleProperty))
                return false;

            if (jsonTitleProperty.ValueKind != JsonValueKind.String)
                return false;

            var title = jsonTitleProperty.GetString();

            if (string.IsNullOrEmpty(title))
                return false;

            // Workaround for missing Contains(string, StringComparison)
            return title.IndexOf(name, StringComparison.InvariantCultureIgnoreCase) >= 0;
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
