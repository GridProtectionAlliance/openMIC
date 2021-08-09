//******************************************************************************************************
//  IniJsonTests.cs - Gbtc
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using static GSF.Collections.CollectionExtensions;
using static openMIC.IniJsonInterop;

namespace DataHubFunctions
{
    [TestClass]
    public class IniJsonTests
    {
        private static string iniInput = 
          @"; ------ PQube 3 from Powerside.
            ; ------ www.powerside.com
            ; ------ PQube 3 Version 3.8
            rootKey=R00tVal
            ;----------------------------------------------------
            [PQube_Information] ; End of section comment
            ;----------------------------------------------------
            ; ------ Assign a unique identifier for your PQube 3
            PQube_ID=""(PQube_ID not set)"" ; End of value comment

            ; ------ Describe the place where your PQube 3 is installed
            Location_Name=""(location not set)""

            ; ------ Optional additional information about your PQube 3
            Note_1=""(note not set)""
            Note_2=""(note not set)""
            [NewSection]
            ; New section
            NewKey=NewValue";

        private static string jsonInput =
          @"{
              ""@c1"": ""; ------ PQube 3 from Powerside."",
              ""@c2"": ""; ------ www.powerside.com"",
              ""@c3"": ""; ------ PQube 3 Version 3.8"",
              ""rootKey"": ""R00tVal"",
              ""@c4"": "";----------------------------------------------------"",
              ""@c5eol"": ""; End of section comment"",
              ""PQube_Information"": {
                ""@c1"": "";----------------------------------------------------"",
                ""@c2"": ""; ------ Assign a unique identifier for your PQube 3"",
                ""@c3eol"": ""; End of value comment"",
                ""PQube_ID"": ""\""(PQube_ID not set)\"""",
                ""@c4"": """",
                ""@c5"": ""; ------ Describe the place where your PQube 3 is installed"",
                ""Location_Name"": ""\""(location not set)\"""",
                ""@c6"": """",
                ""@c7"": ""; ------ Optional additional information about your PQube 3"",
                ""Note_1"": ""\""(note not set)\"""",
                ""Note_2"": ""\""(note not set)\""""
              },
              ""NewSection"": {
                ""@c1"": ""; New section"",
                ""NewKey"": ""NewValue""
              }
            }";

        [TestMethod]
        public void TestIni2Json()
        {
            string json = GetIniAsJson(iniInput, true, true);
            Console.WriteLine(json);
            Assert.IsTrue(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(jsonInput)));
        }

        [TestMethod]
        public void TestJson2Ini()
        {
            string ini = GetJsonAsIni(jsonInput,true);
            Console.WriteLine(ini);

            string[] getLines(string value) {
                return value.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).Select(line => line.Trim()).ToArray();
            }

            string[] sourceLines = getLines(ini);
            string[] targetLines = getLines(iniInput + "\r\n");

            Assert.IsTrue(sourceLines.CompareTo(targetLines) == 0);
        }
    }
}
