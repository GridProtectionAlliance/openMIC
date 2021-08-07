//******************************************************************************************************
//  IniJsonInterop.cs - Gbtc
//
//  Copyright © 2021, Grid Protection Alliance.  All Rights Reserved.
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
//  08/07/2021 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Linq;
using System.Text;
using GSF.Web;
using Newtonsoft.Json.Linq;

namespace openMIC
{
    /// <summary>
    /// Defines interoperability functions for INI and JSON.
    /// </summary>
    public static class IniJsonInterop
    {
        /// <summary>
        /// Gets INI as JSON.
        /// </summary>
        /// <param name="value">INI source.</param>
        /// <param name="indented"><c>true</c> to indent result JSON; otherwise, <c>false</c>.</param>
        /// <param name="preserveComments"><c>true</c> to preserve comments; otherwise, <c>false</c>.</param>
        /// <returns>Converted JSON.</returns>
        public static string GetIniAsJson(string value, bool indented, bool preserveComments)
        {
            StringBuilder json = new();
            string[] lines = value.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).Select(line => line.Trim()).ToArray();
            string section = null;
            int keyValueIndex = 0, commentIndex = 0;

            void addComment(string comment)
            {
                if (!preserveComments)
                    return;

                json.Append($"{(keyValueIndex > 0 ? "," : "")}\"@c{++commentIndex}\":\"{comment.JavaScriptEncode()}\"");
                keyValueIndex++;
            }

            string removeTrailingComment(string line)
            {
                // Remove any trailing comment from section line
                int index = line.IndexOf(";", StringComparison.Ordinal);

                if (index > -1)
                {
                    addComment(line.Substring(index));
                    line = line.Substring(0, index).Trim();
                }

                return line;
            }

            json.Append("{");

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (string.IsNullOrWhiteSpace(line) || line.StartsWith(";"))
                {
                    addComment(line);
                }
                else if (line.StartsWith("["))
                {
                    line = removeTrailingComment(line);

                    if (line.EndsWith("]"))
                    {
                        if (section is not null)
                            json.Append("},");
                        else if (commentIndex > 0)
                            json.Append(",");

                        section = line.Substring(1, line.Length - 2).Trim();
                        json.Append($"\"{section.JavaScriptEncode()}\":{{");
                        keyValueIndex = commentIndex = 0;
                    }
                    else
                    {
                        throw new InvalidOperationException($"INI section has an invalid format: \"{lines[i]}\"");
                    }
                }
                else
                {
                    line = removeTrailingComment(line);

                    string[] kvpParts = line.Split('=');

                    if (kvpParts.Length != 2)
                        throw new InvalidOperationException($"INI key-value entry has an invalid format: \"{lines[i]}\"");

                    json.Append($"{(keyValueIndex > 0 ? "," : "")}\"{kvpParts[0].Trim().JavaScriptEncode()}\":\"{kvpParts[1].Trim().JavaScriptEncode()}\"");
                    keyValueIndex++;
                }
            }

            if (section is not null)
                json.Append("}");

            json.Append("}");

            return indented ?
                JToken.Parse(json.ToString()).ToString(Newtonsoft.Json.Formatting.Indented) :
                json.ToString();
        }

        /// <summary>
        /// Gets JSON as INI.
        /// </summary>
        /// <param name="value">JSON source.</param>
        /// <param name="restoreComments"><c>true</c> to restore comments; otherwise, <c>false</c>.</param>
        /// <returns>Converted INI.</returns>
        public static string GetJsonAsIni(string value, bool restoreComments)
        {
            StringBuilder ini = new();
            JObject json = JObject.Parse(value);

            void writeProperty(JProperty property)
            {
                if (property is null)
                    return;

                if (property.Name.StartsWith("@c"))
                {
                    if (restoreComments)
                        ini.AppendLine(property.Value.ToString());
                }
                else
                {
                    ini.AppendLine($"{property.Name}={property.Value}");
                }
            }

            foreach (JProperty property in json.Properties())
            {
                if (property.Value.HasValues)
                {
                    ini.AppendLine($"[{property.Name}]");

                    foreach (JToken kvp in property.Value)
                        writeProperty(kvp as JProperty);
                }
                else
                {
                    writeProperty(property);
                }
            }

            return ini.ToString();
        }
    }
}