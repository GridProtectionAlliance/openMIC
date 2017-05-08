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
using GSF.Configuration;

namespace BenDownloader
{
    class Program
    {
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
                if (args.Length != 4)
                {
                    Log("Please pass four and only four parameters to BenDownloader...");
                    return;
                }

                BenRunner br = new BenRunner(args[0], args[1], args[2], args[3]);

                if (!br.XferAllFiles())
                    throw new Exception("BEN Downloader failed...");
                
            }
            catch(Exception ex)
            {
                Log("Ben Downloader failed. Message: " + ex);
            }

        }

        #region [Helper Functions]
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
