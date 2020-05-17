// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax.Inlines;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc.Inlines
{
    /// <summary>
    ///     A HTML renderer for a <see cref="LinkInline" />.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{TObject}" />
    public class LinkInlineRenderer : XmlDocObjectRenderer<LinkInline>
    {
        /// <summary>
        ///     Gets or sets a value indicating whether to always add rel="nofollow" for links or not.
        /// </summary>
        public bool AutoRelNoFollow { get; set; }

        protected override void Write(XmlDocRenderer renderer, LinkInline link)
        {
            if (link.IsImage) return;

            var enableHtml = false; // renderer.EnableHtmlForInline;

            if (enableHtml)
            {
                renderer.Write("<a href=\"");
                renderer.WriteEscapeUrl(link.GetDynamicUrl != null ? link.GetDynamicUrl() ?? link.Url : link.Url);
                renderer.Write("\"");
            }
            else
            {
                renderer.Write("<i>");
            }

            if (enableHtml && !string.IsNullOrEmpty(link.Title))
            {
                renderer.Write(" title=\"");
                renderer.WriteEscape(link.Title);
                renderer.Write("\"");
            }

            if (enableHtml)
            {
                if (AutoRelNoFollow) renderer.Write(" rel=\"nofollow\"");
                renderer.Write(">");
            }

            renderer.WriteChildren(link);

            renderer.Write(enableHtml ? "</a>" : "</i>");
        }
    }
}