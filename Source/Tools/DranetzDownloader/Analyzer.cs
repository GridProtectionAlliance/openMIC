//******************************************************************************************************
//  Analyzer.cs - Gbtc
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
//  08/21/2020 - C. Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DranetzDowloader
{
    public class Analyzer
    {
        private DranetzConnection m_connection;
     
        public string Name { get; set; }

        public int Id { get; set; }

        public AnalyzerType Type { get; set; }

        public Analyzer(DranetzConnection connection)
        {
            this.Name = "";
            this.Id = 0;
            this.Type = 0;
            m_connection = connection;
        }

        #region [ methods ]

       
        #endregion
    }

    /// <summary>
    /// The Type of Virtual Analyzer - per Nathan Current only also exists.
    /// </summary>
    public enum AnalyzerType
    {
        Power = 0,
        Voltage_Only = 1,
    }
}
