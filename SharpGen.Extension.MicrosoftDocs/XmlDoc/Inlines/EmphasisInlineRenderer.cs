// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax.Inlines;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc.Inlines
{
    /// <summary>
    ///     A HTML renderer for an <see cref="EmphasisInline" />.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{TObject}" />
    public class EmphasisInlineRenderer : XmlDocObjectRenderer<EmphasisInline>
    {
        /// <summary>
        ///     Delegates to get the tag associated to an <see cref="EmphasisInline" /> object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The HTML tag associated to this <see cref="EmphasisInline" /> object</returns>
        public delegate string GetTagDelegate(EmphasisInline obj);

        /// <summary>
        ///     Initializes a new instance of the <see cref="EmphasisInlineRenderer" /> class.
        /// </summary>
        public EmphasisInlineRenderer() => GetTag = GetDefaultTag;

        /// <summary>
        ///     Gets or sets the GetTag delegate.
        /// </summary>
        public GetTagDelegate GetTag { get; set; }

        protected override void Write(XmlDocRenderer renderer, EmphasisInline obj)
        {
            string tag = null;
            if (renderer.EnableHtmlForInline)
            {
                tag = GetTag(obj);
                renderer.Write("<").Write(tag).Write(">");
            }

            renderer.WriteChildren(obj);
            if (renderer.EnableHtmlForInline) renderer.Write("</").Write(tag).Write(">");
        }

        /// <summary>
        ///     Gets the default HTML tag for ** and __ emphasis.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        private static string GetDefaultTag(EmphasisInline obj)
        {
            if (obj.DelimiterChar != '*' && obj.DelimiterChar != '_')
                return null;

            return obj.DelimiterCount >= 2 ? "b" : "i";
        }
    }
}