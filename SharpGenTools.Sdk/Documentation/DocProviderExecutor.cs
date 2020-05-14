using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SharpGen.CppModel;
using SharpGen.Doc;
using SharpGen.Logging;

namespace SharpGenTools.Sdk.Documentation
{
    internal static class DocProviderExecutor
    {
        public static async Task ApplyDocumentation(IDocProvider docProvider, DocItemCache cache, CppModule module,
                                                    DocumentationContext context)
        {
            var documentationTasks = new List<Task>();

            Task DocumentSelector(CppElement cppElement) =>
                docProvider.DocumentElement(cache, cppElement, context, true, null);

            foreach (var cppInclude in module.Includes)
            {
                documentationTasks.AddRange(cppInclude.Enums.Select(DocumentSelector));
                documentationTasks.AddRange(cppInclude.Structs.Select(DocumentSelector));
                documentationTasks.AddRange(
                    cppInclude.Interfaces
                              .Select(cppInterface => docProvider.DocumentInterface(cache, cppInterface, context))
                );
                documentationTasks.AddRange(
                    cppInclude.Functions
                              .Select(cppFunction => docProvider.DocumentCallable(cache, cppFunction, context))
                );
            }

            await Task.WhenAll(documentationTasks);
        }

        private static async Task<IDocItem> DocumentElement(this IDocProvider docProvider,
                                                           DocItemCache cache,
                                                           CppElement element,
                                                           DocumentationContext context,
                                                           bool documentInnerElements,
                                                           string name)
        {
            var docName = name ?? element.Name;

            if (string.IsNullOrEmpty(docName))
                return null;

            docName = docName.Trim();

            if (string.IsNullOrEmpty(docName))
                return null;

            var cacheEntry = cache.Find(docName);
            var docItem = cacheEntry ?? await QueryDocumentationProvider();

            if (docItem == null)
                return null;

            element.Id = docItem.ShortId;
            element.Description = docItem.Summary;
            element.Remarks = docItem.Remarks;
            docItem.Names.Add(docName);

            if (cacheEntry == null)
                cache.Add(docItem);

            if (element.IsEmpty)
                return docItem;

            if (documentInnerElements)
                DocumentInnerElements(element.Items, docItem);

            return docItem;

            async Task<IDocItem> QueryDocumentationProvider()
            {
                try
                {
                    return await docProvider.FindDocumentationAsync(docName, context);
                }
                catch (Exception e)
                {
                    context.Logger.Error(
                        LoggingCodes.DocumentationProviderInternalError,
                        "Exception occurred during {0} documentation provider query for \"{1}\".",
                        e,
                        docProvider.GetType().Name,
                        docName
                    );

                    return null;
                }
            }
        }

        private static async Task DocumentCallable(this IDocProvider docProvider, DocItemCache cache,
                                                   CppCallable callable, DocumentationContext context,
                                                   string name = null)
        {
            var docItem = await docProvider.DocumentElement(cache, callable, context, true, name);

            if (docItem == null)
                return;

            callable.ReturnValue.Description = docItem.Return;
        }

        private static async Task DocumentInterface(this IDocProvider docProvider, DocItemCache cache,
                                                    CppInterface cppInterface, DocumentationContext context)
        {
            Task DocumentSelector(CppMethod func) =>
                docProvider.DocumentCallable(cache, func, context, cppInterface.Name + "::" + func.Name);

            await Task.WhenAll(
                cppInterface.Methods
                            .Select(DocumentSelector)
                            .Append(docProvider.DocumentElement(cache, cppInterface, context, false, null))
            );
        }

        private static void DocumentInnerElements(IReadOnlyCollection<CppElement> elements, IDocItem docItem)
        {
            var count = Math.Min(elements.Count, docItem.Items.Count);
            var i = 0;
            foreach (var element in elements)
            {
                element.Id = docItem.ShortId;

                // Try to find the matching item
                var foundMatch = false;
                foreach (var subItem in docItem.Items)
                {
                    if (ContainsCppIdentifier(subItem.Term, element.Name))
                    {
                        element.Description = subItem.Description;
                        foundMatch = true;
                        break;
                    }
                }
                if (!foundMatch && i < count)
                    element.Description = docItem.Items[i].Description;
                i++;
            }
        }

        /// <summary>
        /// Determines whether a string contains a given C++ identifier.
        /// </summary>
        /// <param name="str">The string to search.</param>
        /// <param name="identifier">The C++ identifier to search for.</param>
        /// <returns></returns>
        private static bool ContainsCppIdentifier(string str, string identifier)
        {
            if (string.IsNullOrEmpty(str))
                return string.IsNullOrEmpty(identifier);

            return Regex.IsMatch(str, $@"\b{Regex.Escape(identifier)}\b", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }
    }
}
