// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Markdig.Parsers;
using Markdig.Syntax;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc.MonikerRange
{
    public class MonikerRangeBlock : ContainerBlock
    {
        public string MonikerRange { get; set; }
        public int ColonCount { get; set; }
        public bool Closed { get; set; }
        public MonikerRangeBlock(BlockParser parser) : base(parser)
        {
        }
    }
}
