//******************************************************************************************************
//  Program.cs - Gbtc
//
//  Copyright © 2015, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  09/02/2010 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.ServiceProcess;
using System.Web;
using System.Windows.Forms;
using GSF;
using GSF.Console;

namespace openMIC
{
    public static class Program
    {
        /// <summary>
        /// The service host instance for the application.
        /// </summary>
        public static readonly ServiceHost Host = new ServiceHost();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            bool runAsService;
            bool runAsApplication;

            Arguments args = new Arguments(Environment.CommandLine, true);

            if (args.Count > 1)
            {
                MessageBox.Show("Too many arguments. If specified, argument must be one of: -RunAsService, -RunAsApplication or -RunAsConsole.");
                Environment.Exit(1);
            }

            if (args.Count == 0)
            {
#if DEBUG
                runAsService = false;
                runAsApplication = true;
#else
                runAsService = true;
                runAsApplication = false;
#endif
            }
            else
            {
                runAsService = args.Exists("RunAsService");
                runAsApplication = args.Exists("RunAsApplication");

                if (!runAsService && !runAsApplication && !args.Exists("RunAsConsole"))
                {
                    MessageBox.Show("Invalid argument. If specified, argument must be one of: -RunAsService, -RunAsApplication or -RunAsConsole.");
                    Environment.Exit(1);
                }
            }

            if (runAsService)
            {
                // Run as Windows Service.
                ServiceBase.Run(new ServiceBase[] { Host });
            }
            else if (runAsApplication)
            {
                // Run as Windows Application.
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new DebugHost(Host));
            }
            else
            {
                string hostedServiceSessionName = Host.ServiceName + "Shell.exe";
                Process hostedServiceSession = Process.Start(hostedServiceSessionName);

                if ((object)hostedServiceSession != null)
                {
                    hostedServiceSession.WaitForExit();
                    Environment.Exit(hostedServiceSession.ExitCode);
                }
                else
                {
                    MessageBox.Show($"Failed to start \"{hostedServiceSessionName}\" with a hosted service.");
                    Environment.Exit(1);
                }
            }
        }

        /// <summary>
        /// Performs JavaScript encoding on given string.
        /// </summary>
        /// <param name="text">The string to be encoded.</param>
        public static string JavaScriptEncode(this string text)
        {
            return HttpUtility.JavaScriptStringEncode(text.ToNonNullString());
        }

        /// <summary>
        /// Converts a name/value collection to a dictionary.
        /// </summary>
        /// <param name="collection">Name/value collection.</param>
        /// <returns>Dictionary converted from a name/value collection.</returns>
        public static Dictionary<string, string> ToDictionary(this NameValueCollection collection)
        {
            return collection.AllKeys.ToDictionary(key => key, key => collection[key]);
        }

        /// <summary>
        /// Gets query parameters for current request message
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Dictionary<string, string> QueryParameters(this HttpRequestMessage request)
        {
            return request.GetQueryNameValuePairs().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        } 
    }
}