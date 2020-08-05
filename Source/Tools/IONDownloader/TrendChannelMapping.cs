//******************************************************************************************************
//  TrendChannelMapping.cs - Gbtc
//
//  Copyright © 2020, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  07/27/2020 - Stephen C. Wills
//       Generated original version of source code.
//
//******************************************************************************************************

using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Gemstone.IONProtocol;

namespace IONDownloader
{
    public class TrendChannelMapping
    {
        public IONUInt Handle { get; }
        public PQDIFTrendQuantity QuantityDefinition { get; }
        public double ConversionFactor { get; }

        public TrendChannelMapping(IONUInt handle, PQDIFTrendQuantity quantityDefinition, double conversionFactor)
        {
            Handle = handle;
            QuantityDefinition = quantityDefinition;
            ConversionFactor = conversionFactor;
        }

        public static async IAsyncEnumerable<TrendChannelMapping> ReadMappingsAsync(string filePath)
        {
            await using Stream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
            ParsedMapping[] parsedMappings = await JsonSerializer.DeserializeAsync<ParsedMapping[]>(fileStream);

            foreach (ParsedMapping parsedMapping in parsedMappings)
            {
                if (parsedMapping.QuantityDefinition is null)
                    continue;

                yield return new TrendChannelMapping(parsedMapping.Handle, parsedMapping.QuantityDefinition, parsedMapping.ConversionFactor);
            }
        }

        private class ParsedMapping
        {
            public uint Handle { get; set; }
            public PQDIFTrendQuantity? QuantityDefinition { get; set; }
            public double ConversionFactor { get; set; } = 1.0D;
        }
    }
}
