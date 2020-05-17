﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Markdig.Syntax;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc.Interactive
{
    public class CodeSnippetInteractiveRewriter : InteractiveBaseRewriter
    {
        public override IMarkdownObject Rewrite(IMarkdownObject markdownObject)
        {
            if (markdownObject is CodeSnippet.CodeSnippet codeSnippet)
            {
                codeSnippet.Language = GetLanguage(codeSnippet.Language, out bool isInteractive);
                codeSnippet.IsInteractive = isInteractive;

                if (isInteractive)
                {
                    var url = GetGitUrl(codeSnippet);
                    if (!string.IsNullOrEmpty(url))
                    {
                        codeSnippet.GitUrl = url;
                    }

                    return codeSnippet;
                }
            }

            return markdownObject;
        }

        private string GetGitUrl(CodeSnippet.CodeSnippet obj)
        {
            // TODO: Disable to get git URL of code snippet
            return null;
        }
    }
}
