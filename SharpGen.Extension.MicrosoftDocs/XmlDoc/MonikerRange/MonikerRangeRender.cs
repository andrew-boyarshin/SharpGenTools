// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc.MonikerRange
{
    public class MonikerRangeRender : XmlDocObjectRenderer<MonikerRangeBlock>
    {
        protected override void Write(XmlDocRenderer renderer, MonikerRangeBlock obj)
        {
            renderer.WriteLine("<para>");
            renderer.WriteChildren(obj);
            renderer.WriteLine("</para>");
        }
    }
}
