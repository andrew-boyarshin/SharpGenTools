// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Markdig.Parsers;
using Markdig.Syntax;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc.Row
{
    public class RowBlock : ContainerBlock
    {
        public int ColonCount { get; set; }
        public RowBlock(BlockParser parser) : base(parser)
        {
        }
    }
}