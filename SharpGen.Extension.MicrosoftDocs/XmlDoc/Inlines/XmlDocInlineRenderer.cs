// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax.Inlines;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc.Inlines
{
    /// <summary>
    ///     A HTML renderer for a <see cref="HtmlInline" />.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{TObject}" />
    public class XmlDocInlineRenderer : XmlDocObjectRenderer<HtmlInline>
    {
        protected override void Write(XmlDocRenderer renderer, HtmlInline obj)
        {
            if (renderer.EnableHtmlForInline)
                renderer.Write(obj.Tag);
        }
    }
}