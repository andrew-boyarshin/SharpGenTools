// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax.Inlines;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc.Inlines
{
    /// <summary>
    ///     A HTML renderer for a <see cref="CodeInline" />.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{TObject}" />
    public class CodeInlineRenderer : XmlDocObjectRenderer<CodeInline>
    {
        protected override void Write(XmlDocRenderer renderer, CodeInline obj)
        {
            if (renderer.EnableHtmlForInline) renderer.Write("<c>");
            renderer.WriteEscape(obj.Content);
            if (renderer.EnableHtmlForInline) renderer.Write("</c>");
        }
    }
}