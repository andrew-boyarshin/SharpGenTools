﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Markdig.Parsers;
using Markdig.Syntax;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc.TripleColon
{
    public class TripleColonBlock : ContainerBlock
    {
        public IDictionary<string, string> RenderProperties { get; set; }
        public ITripleColonExtensionInfo Extension { get; set; }
        public TripleColonBlock(BlockParser parser) : base(parser) { }
        public bool Closed { get; set; }
        public bool EndingTripleColons { get; set; }
        public IDictionary<string, string> Attributes { get; set; }
    }
}
