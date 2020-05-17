// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Markdig;
using Markdig.Parsers;
using Markdig.Renderers;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc.QuoteSectionNote
{
    public class QuoteSectionNoteExtension : IMarkdownExtension
    {
        private readonly MarkdownContext _context;

        public QuoteSectionNoteExtension(MarkdownContext context)
        {
            _context = context;
        }

        void IMarkdownExtension.Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.BlockParsers.Replace<QuoteBlockParser>(new QuoteSectionNoteParser(_context)))
            {
                pipeline.BlockParsers.Insert(0, new QuoteSectionNoteParser(_context));
            }
        }

        void IMarkdownExtension.Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is XmlDocRenderer htmlRenderer)
            {
                QuoteSectionNoteRender quoteSectionNoteRender = new QuoteSectionNoteRender(_context);

                if (!renderer.ObjectRenderers.Replace<Markdig.Renderers.Html.QuoteBlockRenderer>(quoteSectionNoteRender))
                {
                    renderer.ObjectRenderers.Insert(0, quoteSectionNoteRender);
                }
            }
        }
    }
}
