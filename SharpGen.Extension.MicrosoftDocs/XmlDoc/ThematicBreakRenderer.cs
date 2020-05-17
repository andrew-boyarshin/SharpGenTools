// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax;

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc
{
    /// <summary>
    ///     A HTML renderer for a <see cref="ThematicBreakBlock" />.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{TObject}" />
    public class ThematicBreakRenderer : XmlDocObjectRenderer<ThematicBreakBlock>
    {
        protected override void Write(XmlDocRenderer renderer, ThematicBreakBlock obj)
        {
        }
    }
}