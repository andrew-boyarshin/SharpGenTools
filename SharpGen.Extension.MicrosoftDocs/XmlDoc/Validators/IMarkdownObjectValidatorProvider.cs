// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc.Validators
{
    public interface IMarkdownObjectValidatorProvider
    {
        ImmutableArray<IMarkdownObjectValidator> GetValidators();
    }
}
