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
using System.Collections.Generic;
using System.IO;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GSF;
using GSF.Configuration;
using GSF.Data;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualBasic.FileIO;
using FileSystem = Microsoft.VisualBasic.FileIO.FileSystem;

namespace BenDownloader
{
    class Program
    {
        private static Semaphore s_lock;

        private static object s_logLock = new object();
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
            int setting = s_openMicConfigurationFile.Settings["systemSettings"]["BenRunnerInstanceCount"]?.ValueAsInt32() ?? 0;

            if(setting > 0)
                s_lock = new Semaphore(setting, setting, "BenRunner");

            try
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("Please pass two and only two parameters...");
                    return;
                }

                s_lock?.WaitOne();
                BenRunner br = new BenRunner(int.Parse(args[0]), int.Parse(args[1]));
                if (!br.XferAllFiles())
                {
                    throw new Exception("BEN Downloader failed...");
                }
            }
            catch(Exception ex)
            {
                Log("Ben Downloader failed. Message: " + ex);
            }
            finally
            {   
               
                s_lock?.Release();
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

        public static void Log(string logMessage, string path ="")
        {

            lock (s_logLock)
            {
                try
                {
                    Directory.CreateDirectory(path + "Logs");
                    FileInfo fi = new FileInfo(path + "Logs\\BenDownloaderLogFile.txt");
                    if (fi.Exists && fi.Length > 1048576)
                    {
                        fi.MoveTo(path + $"Logs\\BenDownloaderLogFile[{DateTime.UtcNow.ToString("MM-dd-yy")}].txt");
                    }

                    using (StreamWriter w = File.AppendText(path + "Logs\\BenDownloaderLogFile.txt"))
                    {
                        w.Write("\r\nLog Entry : ");
                        w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                            DateTime.Now.ToLongDateString());
                        w.WriteLine("  :");
                        w.WriteLine("  :{0}", logMessage);
                        w.WriteLine("-------------------------------");
                        w.Flush();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }
        }
        #endregion

    }
}
