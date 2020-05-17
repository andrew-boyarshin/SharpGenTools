// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SharpGen.Extension.MicrosoftDocs.XmlDoc.Aggregator
{

    public interface IBlockAggregator
    {
        bool Aggregate(BlockAggregateContext context);
    }
}
