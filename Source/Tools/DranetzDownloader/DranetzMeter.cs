//******************************************************************************************************
//  DranetzMeter.cs - Gbtc
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

    /// <summary>
    /// This is a connection to a Dranetz meter via IP, Username and Password
    /// </summary>
    public class DranetzMeter: IDisposable
    {
        #region [Fields]
        private DranetzConnection m_connection;
        private List<Analyzer>? m_analyzers = null;
        private List<Session>? m_sessions = null;

        private bool m_disposed;
        #endregion

        #region [ Properties ] 
        public List<Analyzer> Analyzers
        {
            get { return m_analyzers ?? (m_analyzers = GetAnalyzers().Result); }
        }

        public List<Session> Sessions
        {
            get { return m_sessions ?? (m_sessions = GetSession().Result); }
        }

        #endregion

        #region [ Methods ]

        public DranetzMeter(string user, string password, string address, int delay, int delayCount)
        {
            m_connection = new DranetzConnection(user, password, address, delay, delayCount);
            m_disposed = false;
        }

       /// <summary>
       /// Gets the List of Analyzers from the Dranetz Meter.
       /// </summary>
       /// <returns></returns>
        private async Task<List<Analyzer>> GetAnalyzers() 
        {
            XDocument parsedResponse = XDocument.Parse(await m_connection.SendRequest("getinststatus"));

            return parsedResponse.Element("commandresult").Element("analyzers").Element("list").Elements("item").Select(item => new Analyzer(m_connection)
            {
                Id = Convert.ToInt32(item.Attribute("id").Value),
                Name = item.Attribute("name").Value,
                Type = (AnalyzerType)Convert.ToInt32(item.Attribute("analyzer_type").Value)
            }).ToList();
        }

        /// <summary>
        /// Get's the List of Sessions from the Dranetz Meter.
        /// For now we assume there is a single set of sessions - independent of the number of Analyzers
        /// This Assumption might not hold
        /// </summary>
        /// <returns></returns>
        private async Task<List<Session>> GetSession() 
        {

            XDocument parsedResponse = XDocument.Parse(await m_connection.SendRequest($"getdbinfo&id={1}&verbose=1"));
           
                
           List<Session> sessions = parsedResponse.Element("commandresult").Element("list").Elements("item").Select(item => new Session()
           {
               Id = Convert.ToInt32(item.Attribute("session_number").Value),
               Name = item.Attribute("name").Value,
               Time = DateTime.Parse(item.Attribute("session_time").Value),
               Start = Convert.ToInt32(item.Attribute("link_start").Value),
               End = Convert.ToInt32(item.Attribute("link_end").Value),
               NumberofRecords = Convert.ToInt32(item.Attribute("number_of_records").Value)
            }).ToList();

            sessions.ForEach(item => item.SetConnection(m_connection));

            return sessions;
        }

        /// <summary>
        /// Equivalent to calling .Sessions but this is done async
        /// </summary>
        public async Task LoadSessions()
        {
            if (m_sessions == null)
                m_sessions = await GetSession();
        }

        /// <summary>
        /// Equivalent to calling .Analyzers but this is done async
        /// </summary>
        public async Task LoadAnalyzers()
        {
            if (m_analyzers == null)
                m_analyzers = await GetAnalyzers();
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="DranetzMeter"/> object and optionally releases the managed resources.
        /// </summary>
        public void Dispose()
        {
            if (m_disposed)
                return;

            // Dispose of DranetzConnection
            m_connection.Dispose();

            m_disposed = true;
            
        }
        #endregion


    }

}
