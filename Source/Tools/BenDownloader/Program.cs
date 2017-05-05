 //******************************************************************************************************
//  Program.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  10/06/2016 - Billy Ernest
//       Generated original version of source code.
//
//******************************************************************************************************


using System;
using System.IO;
using System.Threading;
using GSF.Configuration;
using GSF.IO;

namespace BenDownloader
{
    class Program
    {
        private static Semaphore s_lock;

        private static readonly object s_logLock = new object();
        private static readonly ConfigurationFile s_openMicConfigurationFile = ConfigurationFile.Open(Directory.GetCurrentDirectory() + "\\openMIC.exe.Config");

        public static ConfigurationFile OpenMiConfigurationFile
        {
            get
            {
                return s_openMicConfigurationFile;
            }
        }

        static void Main(string[] args)
        {


            try
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("Please pass two and only two parameters...");
                    return;
                }

                using (BenRunner br = new BenRunner(int.Parse(args[0]), int.Parse(args[1])))
                {

                    if (!br.XferAllFiles())
                        throw new Exception("BEN Downloader failed...");
                }
            }
            catch(Exception ex)
            {
                Log("Ben Downloader failed. Message: " + ex);
            }

        }

        #region [Helper Functions]
        public static string Left(string str, int length)
        {
            str = (str ?? string.Empty);
            return str.Substring(0, Math.Min(length, str.Length));
        }

        public static string Right(string str, int length)
        {
            str = (str ?? string.Empty);
            return (str.Length >= length)
                ? str.Substring(str.Length - length, length)
                : str;
        }

        public static void Log(string logMessage, bool error = false)
        {
            if(error)
            {
                Console.Error.WriteLine("openMIC: " + logMessage);
            }
            else
            {
                Console.WriteLine("openMIC: " + logMessage);
            }
        }
        #endregion

    }
}
