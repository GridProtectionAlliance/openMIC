//******************************************************************************************************
//  DranetzConnection.cs - Gbtc
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
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DranetzDowloader
{

    /// <summary>
    /// This is a connection to a Dranetz meter via IP, Username and Password
    /// </summary>
    public class DranetzConnection: IDisposable
    {
        private string m_User;
        private string m_Password;
        private string m_IP;
        private string m_token;
        private HttpClient m_client;
        private HttpClientHandler m_conHandler;
        private int m_count;
        private int m_delay;
        private int m_delayCount;
        #region [ Methods ]

        public DranetzConnection(string user, string password, string address, int delay, int delayCount)
        {
            m_User = user;
            m_Password = password;
            m_IP = address;
            m_token = Convert.ToBase64String(Encoding.UTF8.GetBytes((m_User + ":" + (m_Password))));

            m_conHandler = new HttpClientHandler();
            m_conHandler.Credentials = new NetworkCredential(m_User, m_Password);

            m_client = new HttpClient(m_conHandler);
            m_client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization",
                        $"Basic {m_token}");
            m_count = 0;
            m_delay = delay;
            m_delayCount = delayCount;
        }

        public void Dispose()
        {
            m_client.Dispose();
            m_conHandler.Dispose();
        }

        public async Task<string> SendRequest(string command)
        {
            if (m_count > m_delayCount)
            {
                m_count = 0;
                System.Threading.Thread.Sleep(m_delay);
            }
            m_count++;
            return await m_client.GetStringAsync("http://" + m_IP + "/cmd=" + command);
        }

        public async Task<byte[]> SendByteRequest(string command)
        {
            if (m_count > m_delayCount)
            {
                Console.WriteLine($"Stopping Polling Process to avoid locking the meter...");
                m_count = 0;
                System.Threading.Thread.Sleep(m_delay);
                Console.WriteLine($"Continue Polling Process...");
            }
            m_count++;
            return await m_client.GetByteArrayAsync("http://" + m_IP + "/cmd=" + command);
        }
        #endregion

        
    }

}
