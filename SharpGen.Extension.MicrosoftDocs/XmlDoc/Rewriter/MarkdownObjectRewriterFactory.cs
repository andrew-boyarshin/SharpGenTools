// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Markdig.Syntax;
using SharpGen.Extension.MicrosoftDocs.XmlDoc.Validators;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc.Rewriter
{
    public static class MarkdownObjectRewriterFactory
    {
        public static IMarkdownObjectRewriter FromValidators(
            IEnumerable<IMarkdownObjectValidator> validators,
            Action<IMarkdownObject> preProcess = null,
            Action<IMarkdownObject> postProcess = null)
        {
            if (validators == null)
            {
                throw new ArgumentNullException(nameof(validators));
            }

            return new MarkdownObjectValidatorAdapter(validators, preProcess, postProcess);
        }

        public static IMarkdownObjectRewriter FromValidator(
            IMarkdownObjectValidator validator,
            Action<IMarkdownObject> preProcess = null,
            Action<IMarkdownObject> postProcess = null)
        {
            if (validator == null)
            {
                throw new ArgumentNullException(nameof(validator));
            }

            return new MarkdownObjectValidatorAdapter(validator, preProcess, postProcess);
        }
    }
}
