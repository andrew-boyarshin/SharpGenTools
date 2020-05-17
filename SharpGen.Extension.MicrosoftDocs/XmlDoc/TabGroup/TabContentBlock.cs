// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Markdig.Syntax;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc.TabGroup
{
    public class TabContentBlock : ContainerBlock
    {
        public TabContentBlock(List<Block> blocks) : base(null)
        {
            foreach(var item in blocks)
            {
                Add(item);
            }
        }
    }
}