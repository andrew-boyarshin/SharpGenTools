// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Syntax;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc
{
    /// <summary>
    /// A HTML renderer for a <see cref="QuoteBlock"/>.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{TObject}" />
    public class QuoteBlockRenderer : XmlDocObjectRenderer<QuoteBlock>
    {
        protected override void Write(XmlDocRenderer renderer, QuoteBlock obj)
        {
            renderer.EnsureLine();
            if (renderer.EnableHtmlForBlock)
            {
                renderer.Write("<blockquote").WriteLine(">");
            }
            var savedImplicitParagraph = renderer.ImplicitParagraph;
            renderer.ImplicitParagraph = false;
            renderer.WriteChildren(obj);
            renderer.ImplicitParagraph = savedImplicitParagraph;
            if (renderer.EnableHtmlForBlock)
            {
                renderer.WriteLine("</blockquote>");
            }
            renderer.EnsureLine();
        }
    }
}