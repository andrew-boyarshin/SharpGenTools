// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Syntax;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc
{
    /// <summary>
    /// An HTML renderer for a <see cref="CodeBlock"/> and <see cref="FencedCodeBlock"/>.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{TObject}" />
    public class CodeBlockRenderer : XmlDocObjectRenderer<CodeBlock>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeBlockRenderer"/> class.
        /// </summary>
        public CodeBlockRenderer()
        {
        }

        protected override void Write(XmlDocRenderer renderer, CodeBlock obj)
        {
            renderer.EnsureLine();

            if (renderer.EnableHtmlForBlock)
            {
                renderer.Write("<code>");
            }

            renderer.WriteLeafRawLines(obj, true, true);

            if (renderer.EnableHtmlForBlock)
            {
                renderer.WriteLine("</code>");
            }

            renderer.EnsureLine();
        }
    }
}