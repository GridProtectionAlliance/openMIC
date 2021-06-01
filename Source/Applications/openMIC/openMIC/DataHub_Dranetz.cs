//******************************************************************************************************
//  DataHub_Dranetz.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  05/19/2021 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using GSF;
using openMIC.Model;

namespace openMIC
{
    public partial class DataHub
    {
        private static readonly MemoryCache s_dranetzCredentialCache = new MemoryCache($"{nameof(DataHub)}-DranetzCredentialCache");

        private sealed class DranetzCredential : IDisposable
        {
            private readonly SecureString m_rootUri;
            private readonly SecureString m_userName;
            private readonly SecureString m_password;
            private bool m_disposed;

            public DranetzCredential(int deviceID, string rootUri, string userName, string password)
            {
                m_rootUri = rootUri.ToSecureString();
                m_userName = userName.ToSecureString();
                m_password = password.ToSecureString();

                s_dranetzCredentialCache.Add(deviceID.ToString(), this, new CacheItemPolicy
                {
                    SlidingExpiration = TimeSpan.FromSeconds(300.0D),
                    RemovedCallback = args =>
                    {
                        (args.CacheItem.Value as IDisposable)?.Dispose();
                    }
                });
            }

            public void Dispose()
            {
                try
                {
                    if (m_disposed)
                        return;

                    m_rootUri?.Dispose();
                    m_userName?.Dispose();
                    m_password?.Dispose();
                }
                finally
                {
                    m_disposed = true;
                }
            }

            public HttpClient GetNewClient() => new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(Encoding.UTF8.GetBytes(
                        $"{m_userName.ToUnsecureString()}:{m_password.ToUnsecureString()}")))
                }
            };

            public string GetRequestUri(string cmdParam) =>
                $"{m_rootUri.ToUnsecureString()}/cmd={cmdParam}";
        }

        private DranetzCredential GetCredential(int deviceID)
        {
            string key = deviceID.ToString();

            lock (s_dranetzCredentialCache)
            {
                if (s_dranetzCredentialCache.Contains(key))
                    return s_dranetzCredentialCache.Get(key) as DranetzCredential;

                Device device = QueryDeviceByID(deviceID);

                if (device is null)
                    return null;

                Dictionary<string, string> settings = device.ConnectionString.ParseKeyValuePairs();

                if (!settings.TryGetValue("connectionHostName", out string rootUri) || string.IsNullOrEmpty(rootUri))
                    throw new ArgumentException("connectionHostName connection string parameter not found");

                settings.TryGetValue("connectionUserName", out string userName);
                settings.TryGetValue("connectionPassword", out string password);

                if (!rootUri.StartsWith("http"))
                    rootUri = $"http://{rootUri}";

                return new DranetzCredential(deviceID, rootUri, userName, password);
            }

        }

        private void ClearCredential(int deviceID)
        {
            lock (s_dranetzCredentialCache)
                s_dranetzCredentialCache.Remove(deviceID.ToString());
        }

        public Task<string> GetInstanceStatus(int deviceID) => 
            GetCommandResult(deviceID, "getinststatus");

        public Task<string> GetInstanceConfig(int deviceID) =>
            GetCommandResult(deviceID, "getinstconfig");

        public Task<string> GetAnalyzerConfig(int deviceID, int configID) =>
            GetCommandResult(deviceID, $"getanalyzerconfig&id={configID}");

        public Task PutInstanceConfig(int deviceID, string value) =>
            PutCommandResult(deviceID, "putinstconfig", value);

        public Task PutAnalyzerConfig(int deviceID, int configID, string value) =>
            PutCommandResult(deviceID, $"putanalyzerconfig&id={configID}", value);

        private async Task<string> GetCommandResult(int deviceID, string cmdParam)
        {
            try
            {
                DranetzCredential credential = GetCredential(deviceID);

                using (HttpClient client = credential.GetNewClient())
                {
                    HttpResponseMessage result = await client.GetAsync(credential.GetRequestUri(cmdParam));

                    if (!result.IsSuccessStatusCode)
                        throw new HttpRequestException($"Failed to get command result: status response code {result.StatusCode} - {result.ReasonPhrase}");

                    XmlDocument commandResult = new XmlDocument();
                    commandResult.Load(await result.Content.ReadAsStreamAsync());
                    return JsonConvert.SerializeXmlNode(commandResult);
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                return default;
            }
        }

        private async Task PutCommandResult(int deviceID, string cmdParam, string value)
        {
            try
            {
                DranetzCredential credential = GetCredential(deviceID);

                using (HttpClient client = credential.GetNewClient())
                {
                    XmlDocument commandResult = JsonConvert.DeserializeXmlNode(value);
                    StringContent content = new StringContent(commandResult.ToString(), Encoding.UTF8, "text/xml");
                    HttpResponseMessage result = await client.PostAsync(credential.GetRequestUri(cmdParam), content);

                    if (!result.IsSuccessStatusCode)
                        throw new HttpRequestException($"Failed to put command result: status response code {result.StatusCode} - {result.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }
    }
}