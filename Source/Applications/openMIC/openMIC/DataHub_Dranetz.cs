﻿//******************************************************************************************************
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
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using GSF;
using GSF.Units;
using GSF.Web.Security;
using openMIC.Model;

namespace openMIC;

public partial class DataHub
{
    private static readonly MemoryCache s_dranetzCredentialCache = new($"{nameof(DataHub)}-DranetzCredentialCache");

    private sealed class DranetzCredential : IDisposable
    {
        private readonly SecureString m_rootUri;
        private readonly SecureString m_userName;
        private readonly SecureString m_password;
        private bool m_disposed;

        public DranetzCredential(string rootUri, string userName, string password)
        {
            m_rootUri = rootUri.ToSecureString();
            m_userName = userName.ToSecureString();
            m_password = password.ToSecureString();
        }

        public CookieContainer Cookies { get; private set; } = new();

        public Uri RootUri => new(m_rootUri.ToUnsecureString());

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
            HttpClient client =  new(new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = Cookies
            }, true)
            {
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue("Basic", 
                        Convert.ToBase64String(Encoding.UTF8.GetBytes(
                        $"{m_userName.ToUnsecureString()}:{m_password.ToUnsecureString()}")))
                }
            };

            string[] cookies = Cookies.GetCookies(RootUri).Cast<Cookie>()
                                      .Select(cookie => $"{cookie.Name}={cookie.Value}").ToArray();

            if (cookies.Length > 0)
                client.DefaultRequestHeaders.Add("Cookie", string.Join(",", cookies));

            return client;
        }

        public void ClearCookies() =>
            Cookies = new CookieContainer();

        public string GetRequestUri(string cmdParam) =>
            $"{m_rootUri.ToUnsecureString()}/cmd={cmdParam}";
    }

    private DranetzCredential DrantezGetCredential(int deviceID)
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

            // Strip off any analyzer specifications, this assumes "ip:port/analyzer" format
            string[] parts = rootUri.Split('/');

            if (parts.Length == 2)
                rootUri = parts[0];

            if (!rootUri.StartsWith("http"))
                rootUri = $"http://{rootUri}";

            DranetzCredential credential = new(rootUri, userName, password);

            s_dranetzCredentialCache.Add(deviceID.ToString(), credential, new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromSeconds(300.0D),
                RemovedCallback = args =>
                {
                    (args.CacheItem.Value as IDisposable)?.Dispose();
                }
            });
                
            return credential;
        }
    }

    private void DranetzClearCredential(int deviceID)
    {
        lock (s_dranetzCredentialCache)
            s_dranetzCredentialCache.Remove(deviceID.ToString());
    }

    public Task<string> DrantezGetInstanceStatus(int deviceID) => 
        DrantezGetCommandJson(deviceID, "getinststatus");

    public Task<string> DrantezGetInstanceConfig(int deviceID) =>
        DrantezGetCommandJson(deviceID, "getinstconfig");

    public Task<string> DrantezGetAnalyzerConfig(int deviceID, int configID) =>
        DrantezGetCommandJson(deviceID, $"getanalyzerconfig&id={configID}");

    // Sets TransactionID cookie
    [AuthorizeHubRole("Administrator, Editor")]
    public Task DrantezBeginTransaction(int deviceID) =>
        DrantezGetCommandJson(deviceID, "begintransaction");

    [AuthorizeHubRole("Administrator, Editor")]
    public Task DrantezSetInstanceConfig(int deviceID, string value) =>
        DrantezPostCommandJson(deviceID, "setinstconfig", value);

    [AuthorizeHubRole("Administrator, Editor")]
    public Task DrantezSetAnalyzerConfig(int deviceID, int configID, string value) =>
        DrantezPostCommandJson(deviceID, $"setanalyzerconfig&id={configID}", value);

    // Clears TransactionID cookie
    [AuthorizeHubRole("Administrator, Editor")]
    public Task DrantezEndTransaction(int deviceID) =>
        DrantezGetCommandJson(deviceID, "endtransaction", true);

    public Task<string> DrantezGetDeviceTime(int deviceID) =>
        DrantezGetCommandJson(deviceID, "gettime");
        
    public async Task<dynamic> DrantezGetDeviceTimeWithError(int deviceID)
    {
        XmlDocument result = await DrantezGetCommandXml(deviceID, "gettime");
        string receivedTime = result.SelectSingleNode("commandresult/characteristics/@current_time")?.Value ?? throw new NullReferenceException("Failed to get device time");
        DateTime parsedTime = DateTime.ParseExact(receivedTime, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
        double totalError = (parsedTime - DateTime.UtcNow).TotalSeconds;

        return new
        {
            time = parsedTime.ToString(ShortDateTimeFormat),
            error = totalError,
            errorText = Time.ToElapsedTimeString(Math.Abs(totalError), 2)
        };
    }

    [AuthorizeHubRole("Administrator, Editor")]
    public Task DrantezSetDeviceTime(int deviceID, string time) =>
        DrantezGetCommandJson(deviceID, $"puttime&time={DateTime.Parse(time, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal):MM/dd/yyyy HH:mm:ss}");

    [AuthorizeHubRole("Administrator, Editor")]
    public Task DrantezRestartDevice(int deviceID, bool hard) =>
        DrantezGetCommandJson(deviceID, $"restart&hard={(hard ? "yes" : "no")}");

    public Task<string> DrantezGetValuesShortList(int deviceID, int configID) =>
        DrantezGetValues(deviceID, configID, 0, 54);

    public Task<string> DrantezGetValuesLongList(int deviceID, int configID) =>
        DrantezGetValues(deviceID, configID, 0, 3234);

    public Task<string> DrantezGetValues(int deviceID, int configID, int lowRegister, int highRegister) =>
        DrantezGetCommandJson(deviceID, $"getvalues&id={configID}&registers={lowRegister}-{highRegister}&verbose=1");

    public Task<string> DrantezGetWaveforms(int deviceID, int configID) =>
        DrantezGetCommandJson(deviceID, $"getwaveforms&id={configID}&format=0");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async Task<XmlDocument> DrantezGetCommandXml(int deviceID, string cmdParam, bool clearCookies = false)
    {
        if (string.IsNullOrWhiteSpace(cmdParam))
            throw new ArgumentNullException(nameof(cmdParam));

        DranetzCredential credential = DrantezGetCredential(deviceID);
        using HttpClient client = credential.GetNewClient();
        HttpResponseMessage result = await client.GetAsync(credential.GetRequestUri(cmdParam));

        if (!result.IsSuccessStatusCode)
            throw new HttpRequestException($"Failed to get command, result = {result.StatusCode}: {result.ReasonPhrase}");

        XmlDocument commandResult = new();
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

        return commandResult;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async Task<string> DrantezGetCommandJson(int deviceID, string cmdParam, bool clearCookies = false) => 
        JsonConvert.SerializeXmlNode(await DrantezGetCommandXml(deviceID, cmdParam, clearCookies));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async Task DrantezPostCommandXml(int deviceID, string cmdParam, XmlDocument commandResult)
    {
        if (string.IsNullOrWhiteSpace(cmdParam))
            throw new ArgumentNullException(nameof(cmdParam));

        if (commandResult is null)
            throw new ArgumentNullException(nameof(commandResult));

        DranetzCredential credential = DrantezGetCredential(deviceID);

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

        using HttpClient client = credential.GetNewClient();
        StringContent content = new(commandResult.InnerXml, Encoding.UTF8, "text/xml");
        HttpResponseMessage result = await client.PostAsync(credential.GetRequestUri(cmdParam), content);

        if (!result.IsSuccessStatusCode)
            throw new HttpRequestException($"Failed to send command, result = {result.StatusCode}: {result.ReasonPhrase}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Task DrantezPostCommandJson(int deviceID, string cmdParam, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value));

        XmlDocument commandResult = JsonConvert.DeserializeXmlNode(value) ?? throw new ArgumentException("Failed to deserialize XML Node", nameof(value));

        return DrantezPostCommandXml(deviceID, cmdParam, commandResult);
    }
}