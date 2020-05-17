// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc.TripleColon
{
    public class TripleColonRenderer : XmlDocObjectRenderer<TripleColonBlock>
    {
        protected override void Write(XmlDocRenderer renderer, TripleColonBlock b)
        {
            if (b.Extension.Render(renderer, b))
            {
                return;
            }

            renderer.WriteLine("<para>");
            renderer.WriteChildren(b);
            renderer.WriteLine("</para>");
        }
    }
}
