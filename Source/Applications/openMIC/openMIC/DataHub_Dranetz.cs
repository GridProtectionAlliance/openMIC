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
using System.IO;
using System.Linq;
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

            public CookieContainer Cookies { get; private set; } = new CookieContainer();

            public Uri RootUri => new Uri(m_rootUri.ToUnsecureString());

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

            public HttpClient GetNewClient()
            {
                HttpClient client =  new HttpClient(new HttpClientHandler
                {
                    UseCookies = true,
                    CookieContainer = Cookies
                }, true)
                {
                    DefaultRequestHeaders =
                    {
                        Authorization = new AuthenticationHeaderValue("Basic", 
                            Convert.ToBase64String(Encoding.UTF8.GetBytes($"{m_userName.ToUnsecureString()}:{m_password.ToUnsecureString()}"))),
                    }
                };

                List<string> cookies = new List<string>();

                foreach (Cookie cookie in Cookies.GetCookies(RootUri))
                    cookies.Add($"{cookie.Name}={cookie.Value}");

                if (cookies.Count > 0)
                    client.DefaultRequestHeaders.Add("Cookie", string.Join(",", cookies));

                return client;
            }

            public void ClearCookies() =>
                Cookies = new CookieContainer();

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

        public string GetSectionMap(string mapName)
        {
            string mapFileName = Path.Combine(WebRootPath, "SectionMaps", mapName);

            if (!File.Exists(mapFileName))
                throw new FileNotFoundException("Section Map Not Found", mapName);

            XmlDocument mapFile = new XmlDocument();
            mapFile.Load(mapFileName);
            
            return JsonConvert.SerializeXmlNode(mapFile);
        }

        public Task<string> GetInstanceStatus(int deviceID) => 
            GetCommand(deviceID, "getinststatus");

        public Task<string> GetInstanceConfig(int deviceID) =>
            GetCommand(deviceID, "getinstconfig");

        public Task<string> GetAnalyzerConfig(int deviceID, int configID) =>
            GetCommand(deviceID, $"getanalyzerconfig&id={configID}");

        public Task BeginTransaction(int deviceID) =>
            GetCommand(deviceID, "begintransaction"); // Sets TransactionID cookie

        public Task SetInstanceConfig(int deviceID, string value) =>
            SendCommand(deviceID, "setinstconfig", value);

        public Task SetAnalyzerConfig(int deviceID, int configID, string value) =>
            SendCommand(deviceID, $"setanalyzerconfig&id={configID}", value);

        public Task EndTransaction(int deviceID) =>
            GetCommand(deviceID, "endtransaction", true); // Clears TransactionID cookie

        private async Task<string> GetCommand(int deviceID, string cmdParam, bool clearCookies = false)
        {
            if (string.IsNullOrWhiteSpace(cmdParam))
                throw new ArgumentNullException(nameof(cmdParam));

            DranetzCredential credential = GetCredential(deviceID);

            using (HttpClient client = credential.GetNewClient())
            {
                HttpResponseMessage result = await client.GetAsync(credential.GetRequestUri(cmdParam));

                if (!result.IsSuccessStatusCode)
                    throw new HttpRequestException($"Failed to get command, result = {result.StatusCode}: {result.ReasonPhrase}");

                XmlDocument commandResult = new XmlDocument();
                commandResult.Load(await result.Content.ReadAsStreamAsync());

                if (clearCookies)
                {
                    credential.ClearCookies();
                }
                else
                {
                    Uri rootUri = credential.RootUri;
                    string[] cookies = result.Headers.SingleOrDefault(header => 
                        header.Key.Equals("Set-Cookie", StringComparison.OrdinalIgnoreCase)).Value?
                        .ToArray() ?? Array.Empty<string>();

                    foreach (string cookie in cookies)
                    {
                        string[] parts = cookie.Split('=');

                        if (parts.Length == 2 && parts[0].Trim().Equals("TransactionID", StringComparison.OrdinalIgnoreCase))
                            credential.Cookies.SetCookies(rootUri, cookie);
                    }
                }
                    
                return JsonConvert.SerializeXmlNode(commandResult);
            }
        }

        private async Task SendCommand(int deviceID, string cmdParam, string value)
        {
            if (string.IsNullOrWhiteSpace(cmdParam))
                throw new ArgumentNullException(nameof(cmdParam));

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            DranetzCredential credential = GetCredential(deviceID);
            XmlDocument commandResult = JsonConvert.DeserializeXmlNode(value) ?? throw new ArgumentException("Failed to deserialize XML Node", nameof(value));

            if (commandResult.FirstChild is XmlDeclaration declaration)
            {
                declaration.Encoding = Encoding.UTF8.HeaderName;
            }
            else
            {
                declaration = (XmlDeclaration)commandResult.CreateNode(XmlNodeType.XmlDeclaration, "xml", string.Empty);
                declaration.Encoding = Encoding.UTF8.HeaderName;
                commandResult.InsertBefore(declaration, commandResult.FirstChild);
            }

            using (HttpClient client = credential.GetNewClient())
            {
                StringContent content = new StringContent(commandResult.InnerXml, Encoding.UTF8, "text/xml");
                HttpResponseMessage result = await client.PostAsync(credential.GetRequestUri(cmdParam), content);

                if (!result.IsSuccessStatusCode)
                    throw new HttpRequestException($"Failed to send command, result = {result.StatusCode}: {result.ReasonPhrase}");
            }
        }
    }
}