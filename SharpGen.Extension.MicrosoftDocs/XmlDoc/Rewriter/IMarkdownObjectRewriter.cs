// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Markdig.Syntax;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc.Rewriter
{
    public interface IMarkdownObjectRewriter
    {
        void PreProcess(IMarkdownObject markdownObject);

        IMarkdownObject Rewrite(IMarkdownObject markdownObject);

        void PostProcess(IMarkdownObject markdownObject);
    }
}