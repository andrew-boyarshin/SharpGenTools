// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc.TabGroup
{
    public class HtmlTabTitleBlockRenderer : XmlDocObjectRenderer<TabTitleBlock>
    {
        protected override void Write(XmlDocRenderer renderer, TabTitleBlock block)
        {
            foreach(var inline in block.Inline)
            {
                renderer.Render(inline);
            }
        }
    }
}
