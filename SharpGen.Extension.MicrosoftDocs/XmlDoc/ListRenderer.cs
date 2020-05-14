// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Syntax;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc
{
    /// <summary>
    /// A HTML renderer for a <see cref="ListBlock"/>.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{TObject}" />
    public class ListRenderer : XmlDocObjectRenderer<ListBlock>
    {
        protected override void Write(XmlDocRenderer renderer, ListBlock listBlock)
        {
            renderer.EnsureLine();
            if (renderer.EnableHtmlForBlock)
            {
                var listType = listBlock.IsOrdered ? "number" : "bullet";
                renderer.WriteLine($"<list type=\"{listType}\">");
            }

            foreach (var item in listBlock)
            {
                var listItem = (ListItemBlock)item;
                var previousImplicit = renderer.ImplicitParagraph;
                renderer.ImplicitParagraph = !listBlock.IsLoose;

                renderer.EnsureLine();
                if (renderer.EnableHtmlForBlock)
                {
                    renderer.Write("<item><description>");
                }

                renderer.WriteChildren(listItem);

                if (renderer.EnableHtmlForBlock)
                {
                    renderer.WriteLine("</description></item>");
                }

                renderer.EnsureLine();
                renderer.ImplicitParagraph = previousImplicit;
            }

            if (renderer.EnableHtmlForBlock)
            {
                renderer.WriteLine("</list>");
            }

            renderer.EnsureLine();
        }
    }
}