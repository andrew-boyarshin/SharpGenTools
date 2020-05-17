// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc.Row
{
    public class RowRender : XmlDocObjectRenderer<RowBlock>
    {
        protected override void Write(XmlDocRenderer renderer, RowBlock obj)
        {
            renderer.WriteLine("<section class=\"row\">");
            renderer.WriteChildren(obj);
            renderer.WriteLine("</section>");
        }
    }
}
