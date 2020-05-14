// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Syntax.Inlines;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc.Inlines
{
    /// <summary>
    /// A HTML renderer for a <see cref="HtmlEntityInline"/>.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{TObject}" />
    public class XmlDocEntityInlineRenderer : XmlDocObjectRenderer<HtmlEntityInline>
    {
        protected override void Write(XmlDocRenderer renderer, HtmlEntityInline obj)
        {
            if (renderer.EnableHtmlEscape)
            {
                renderer.WriteEscape(obj.Transcoded);
            }
            else
            {
                renderer.Write(obj.Transcoded);
            }
        }
    }
}
