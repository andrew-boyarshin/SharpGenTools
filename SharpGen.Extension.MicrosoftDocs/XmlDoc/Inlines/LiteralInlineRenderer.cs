// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Syntax.Inlines;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc.Inlines
{
    /// <summary>
    /// A HTML renderer for a <see cref="LiteralInline"/>.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{TObject}" />
    public class LiteralInlineRenderer : XmlDocObjectRenderer<LiteralInline>
    {
        protected override void Write(XmlDocRenderer renderer, LiteralInline obj)
        {
            if (renderer.EnableHtmlEscape)
            {
                renderer.WriteEscape(ref obj.Content);
            }
            else
            {
                renderer.Write(ref obj.Content);
            }
        }
    }
}
