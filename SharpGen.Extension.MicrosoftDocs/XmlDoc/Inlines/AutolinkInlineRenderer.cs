// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax.Inlines;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc.Inlines
{
    /// <summary>
    ///     A HTML renderer for an <see cref="AutolinkInline" />.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{TObject}" />
    public class AutolinkInlineRenderer : XmlDocObjectRenderer<AutolinkInline>
    {
        /// <summary>
        ///     Gets or sets a value indicating whether to always add rel="nofollow" for links or not.
        /// </summary>
        public bool AutoRelNoFollow { get; set; }

        protected override void Write(XmlDocRenderer renderer, AutolinkInline obj)
        {
            var enableHtml = false; // renderer.EnableHtmlForInline;

            if (enableHtml)
            {
                renderer.Write("<a href=\"");
                if (obj.IsEmail) renderer.Write("mailto:");
                renderer.WriteEscapeUrl(obj.Url);
                renderer.Write('"');

                if (!obj.IsEmail && AutoRelNoFollow) renderer.Write(" rel=\"nofollow\"");

                renderer.Write(">");
            }
            else
            {
                renderer.Write("<i>");
            }

            renderer.WriteEscape(obj.Url);

            renderer.Write(enableHtml ? "</a>" : "</i>");
        }
    }
}