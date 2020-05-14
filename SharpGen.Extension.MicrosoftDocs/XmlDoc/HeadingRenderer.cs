// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Globalization;
using Markdig.Renderers;
using Markdig.Syntax;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc
{
    /// <summary>
    /// An HTML renderer for a <see cref="HeadingBlock"/>.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{TObject}" />
    public class HeadingRenderer : XmlDocObjectRenderer<HeadingBlock>
    {
        protected override void Write(XmlDocRenderer renderer, HeadingBlock obj)
        {
            var headingText = obj.Level.ToString(CultureInfo.InvariantCulture);

            if (renderer.EnableHtmlForBlock)
            {
                renderer.Write("<h").Write(headingText).Write(">");
            }

            renderer.WriteLeafInline(obj);

            if (renderer.EnableHtmlForBlock)
            {
                renderer.Write("</h").Write(headingText).WriteLine(">");
            }

            renderer.EnsureLine();
        }
    }
}