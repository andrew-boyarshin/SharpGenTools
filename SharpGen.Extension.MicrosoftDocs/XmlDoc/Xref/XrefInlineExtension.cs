// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Markdig;
using Markdig.Parsers.Inlines;
using Markdig.Renderers;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc.Xref
{
    public class XrefInlineExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.InlineParsers.InsertBefore<AutolineInlineParser>(new XrefInlineParser());
            pipeline.InlineParsers.AddIfNotAlready(new XrefInlineShortParser());
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is XmlDocRenderer htmlRenderer && !htmlRenderer.ObjectRenderers.Contains<HtmlXrefInlineRender>())
            {
                // Must be inserted before CodeBlockRenderer
                htmlRenderer.ObjectRenderers.Insert(0, new HtmlXrefInlineRender());
            }
        }
    }
}
