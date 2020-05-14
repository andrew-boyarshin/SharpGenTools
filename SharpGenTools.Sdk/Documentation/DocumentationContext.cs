using System;
using SharpGen.Doc;
using SharpGen.Logging;

namespace SharpGenTools.Sdk.Documentation
{
    public class DocumentationContext : IDocumentationContext
    {
        public DocumentationContext(Logger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IDocItem CreateItem() => new DocItem();

        public IDocSubItem CreateSubItem() => new DocSubItem();

        public Logger Logger { get; }
    }
}