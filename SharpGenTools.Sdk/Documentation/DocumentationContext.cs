using System;
using SharpGen.Doc;
using SharpGen.Logging;
using SharpGenTools.Sdk.Internal;

namespace SharpGenTools.Sdk.Documentation
{
    public class DocumentationContext : IDocumentationContext
    {
        public DocumentationContext(Logger logger) =>
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));

        internal ObservableSet<DocumentationQueryFailure> Failures { get; } =
            new ObservableSet<DocumentationQueryFailure>(DocumentationQueryFailure.QueryComparer);

        public IDocItem CreateItem() => new DocItem();

        public IDocSubItem CreateSubItem() => new DocSubItem();

        public Logger Logger { get; }
    }
}