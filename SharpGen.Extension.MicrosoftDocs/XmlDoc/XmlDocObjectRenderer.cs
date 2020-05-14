// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc
{
    /// <summary>
    /// A base class for HTML rendering <see cref="Block"/> and <see cref="Inline"/> Markdown objects.
    /// </summary>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    /// <seealso cref="IMarkdownObjectRenderer" />
    public abstract class XmlDocObjectRenderer<TObject> : MarkdownObjectRenderer<XmlDocRenderer, TObject> where TObject : MarkdownObject
    {
    }
}