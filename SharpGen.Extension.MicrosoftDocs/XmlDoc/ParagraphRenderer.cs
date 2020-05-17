// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc
{
    /// <summary>
    ///     A HTML renderer for a <see cref="ParagraphBlock" />.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{TObject}" />
    public class ParagraphRenderer : XmlDocObjectRenderer<ParagraphBlock>
    {
        protected override void Write(XmlDocRenderer renderer, ParagraphBlock obj)
        {
            if (!renderer.ImplicitParagraph)
            {
                if (!renderer.IsFirstInContainer) renderer.EnsureLine();

                renderer.Write("<para>");
            }

            renderer.WriteLeafInline(obj);
            if (!renderer.ImplicitParagraph)
            {
                renderer.WriteLine("</para>");

                renderer.EnsureLine();
            }
        }
    }
}