//******************************************************************************************************
//  BenRunner.cs - Gbtc
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using GSF;
using GSF.Collections;
using GSF.Configuration;

namespace BenDownloader
{
    public class BenRunner
    {
        #region [ Members ]

        private const long BENMAXFILESIZE = 7000;
        private const int MAXFILELIMIT = 100;

        private const string BenReqFileName = "benlink.req";
        private const string BenRspFileName = "benlink.rsp";
        private const string BenDirFileName = "bendir.txt";
        private const string BenLogFileName = "benlog.txt";

        private readonly string m_ipAddress;
        private readonly string m_localPath;
        private readonly string m_siteName;
        private readonly string m_serialNumber;
        private readonly string m_tempDirectoryPath;
        private readonly BenRecord m_lastFileDownloaded;
        private readonly int m_maxRetriesOnFileInUse;

        #endregion

        #region [ Constructors ]

        public BenRunner(string localPath, string siteName, string ip, string serialNumber)
        {
            m_localPath = localPath;
            m_siteName = siteName;
            m_ipAddress = ip;
            m_serialNumber = serialNumber;

            Directory.CreateDirectory(m_localPath);
            m_lastFileDownloaded = GetLastDownloadedFile();
            m_maxRetriesOnFileInUse = ConfigurationFile.Current.Settings["systemSettings"]["MaxRetriesOnFileInUse"]?.ValueAsInt32() ?? 8;

            m_tempDirectoryPath = Path.Combine(Path.GetTempPath(), "BenDownloader", m_siteName);

            if (Directory.Exists(m_tempDirectoryPath))
                Directory.Delete(m_tempDirectoryPath, true);

            Directory.CreateDirectory(m_tempDirectoryPath);
        }

        #endregion

        #region [ Methods ]

        public void TransferAllFiles()
        {
            List<BenRecord> myFiles = GetFileList();

            if (!myFiles.Any())
            {
                Program.Log("File list retrieved. There are no files to download.");
                return;
            }

            Program.Log($"File list retrieved. Proceeding to download {myFiles.Count} records.");
            string workingDirectory = Path.Combine(m_tempDirectoryPath, "benfiles.req");
            BuildBenLinkDLINI(workingDirectory, myFiles);
            ExecBenCommand(workingDirectory);
            UpdateTimestampsAndMoveFiles(workingDirectory);
        }

        private List<BenRecord> GetFileList()
        {
            List<BenRecord> downloadList = new List<BenRecord>();
            string workingDirectory = Path.Combine(m_tempDirectoryPath, "bendir.req");
            string dirFilePath = Path.Combine(workingDirectory, BenDirFileName);
            string logFilePath = Path.Combine(workingDirectory, BenLogFileName);

            // Delete the existing dir file if one exists
            if (File.Exists(dirFilePath))
                File.Delete(dirFilePath);

            // Build new dir file
            Program.Log("Requesting list of files to be downloaded from the device...");
            BuildBenLinkDirINI(workingDirectory);

            int retries = 0;

            while (true)
            {
                ExecBenCommand(workingDirectory);

                if (File.Exists(dirFilePath))
                    break;

                string lastLogEntry = File.ReadAllLines(logFilePath).LastOrDefault() ?? string.Empty;
                string errorMessage = Regex.Match(lastLogEntry, "ERROR.*", RegexOptions.IgnoreCase).Value;
                bool fileInUse = Regex.IsMatch(errorMessage, "FILE ALREADY IN USE", RegexOptions.IgnoreCase);

                if (!fileInUse)
                    throw new Exception($"Error received from benlink: {errorMessage}");

                if (m_maxRetriesOnFileInUse >= 0 && retries++ > m_maxRetriesOnFileInUse)
                    throw new Exception("Exceeded maximum number of benlink retries");

                Program.Log($"Encountered FILE IN USE error; initiating retry attempt {retries}");
            }

            // Build list of records to download
            using (TextReader dirFileReader = File.OpenText(dirFilePath))
            {
                string line;

                while ((line = dirFileReader.ReadLine()) != null)
                {
                    string[] curRow = line.Split('\t');

                    if (Convert.ToInt32(curRow[2]) >= BENMAXFILESIZE)
                    {
                        Program.Log("File too large Error: " + m_siteName + " - " + Convert.ToString(curRow[0]), true);
                        continue;
                    }

                    BenRecord record = new BenRecord(Convert.ToInt32(curRow[0]), Convert.ToDateTime(curRow[1]), Convert.ToInt32(curRow[2]), Get232Fn(Convert.ToDateTime(curRow[1]), curRow[0]));

                    if (record.DateTime > DateTime.UtcNow.AddDays(-30) && record.DateTime > m_lastFileDownloaded.DateTime && !File.Exists(Path.Combine(m_localPath, record.Name)))
                        downloadList.Add(record);
                }
            }

            return downloadList;
        }

        private void BuildBenLinkDirINI(string workingDirectory)
        {
            string requestFileName = Path.Combine(workingDirectory, BenReqFileName);

            StringBuilder iniFileBuilder = new StringBuilder()
                .AppendLine($"[Signature]")
                .AppendLine($"Program=BenLink")
                .AppendLine($"FileType=Request")
                .AppendLine($"FileVersion=1")
                .AppendLine()
                .AppendLine($"[General]")
                .AppendLine($"NbRequest=1")
                .AppendLine()
                .AppendLine($"[Device]")
                .AppendLine($"DeviceType=5")
                .AppendLine($"DeviceSN={m_serialNumber}")
                .AppendLine($"NominalFrequency=60")
                .AppendLine($"DataDirectory={workingDirectory}")
                .AppendLine()
                .AppendLine($"[ConnectionParam]")
                .AppendLine($"AccessType=0")
                .AppendLine($"UserName=0")
                .AppendLine($"CommAddress=1")
                .AppendLine($"IPAddress={m_ipAddress}")
                .AppendLine($"HangupTimeout=0")
                .AppendLine()
                .AppendLine($"[Request1]")
                .AppendLine($"RequestType=1")
                .AppendLine($"Origin=1")
                .AppendLine($"SubBens=1")
                .AppendLine($"DataPath={workingDirectory}");

            FileInfo file = new FileInfo(requestFileName);
            file.Directory.Create();
            File.WriteAllText(file.FullName, iniFileBuilder.ToString(), Encoding.ASCII);
        }

        private void BuildBenLinkDLINI(string workingDirectory, List<BenRecord> fileList)
        {
            string requestfilename = Path.Combine(workingDirectory, BenReqFileName);

            StringBuilder iniFileBuilder = new StringBuilder()
                .AppendLine($"[Signature]")
                .AppendLine($"Program=BenLink")
                .AppendLine($"FileType=Request")
                .AppendLine($"FileVersion=1")
                .AppendLine()
                .AppendLine($"[General]")
                .AppendLine($"NbRequest={fileList.Count}")
                .AppendLine()
                .AppendLine($"[Device]")
                .AppendLine($"DeviceType=5")
                .AppendLine($"DeviceSN={m_serialNumber}")
                .AppendLine($"NominalFrequency=60")
                .AppendLine($"DataDirectory={workingDirectory}")
                .AppendLine()
                .AppendLine($"CommAddress=1")
                .AppendLine($"[ConnectionParam]")
                .AppendLine($"AccessType=0")
                .AppendLine($"UserName=0")
                .AppendLine($"IPAddress={m_ipAddress}")
                .AppendLine($"HangupTimeout=0");

            int i = 1;

            foreach (BenRecord currec in fileList)
            {
                iniFileBuilder
                    .AppendLine()
                    .AppendLine($"[Request{i++}]")
                    .AppendLine($"RequestType=2")
                    .AppendLine($"RecordNum={currec.Id}")
                    .AppendLine($"SubBenNum=0")
                    .AppendLine($"Origin=1")
                    .AppendLine($"OptionFlags=2")
                    .AppendLine($"DataPath={workingDirectory}")
                    .AppendLine($"FileName={currec.Name}");
            }

            FileInfo file = new FileInfo(requestfilename);
            file.Directory.Create();
            File.WriteAllText(file.FullName, iniFileBuilder.ToString(), Encoding.ASCII);
        }

        private void ExecBenCommand(string workingDirectory)
        {
            string benLinCmdLine = ConfigurationFile.Current.Settings["systemSettings"]["BenlinkCommandLine"].Value;
            string benLogFilePath = Path.Combine(workingDirectory, BenLogFileName);
            string cmdLine = string.Format(benLinCmdLine, GetShortPath(workingDirectory), GetShortPath(benLogFilePath));
            string[] cmdLineSplit = cmdLine.Split(new char[] { ' ' }, 2);

            ProcessStartInfo psi = new ProcessStartInfo(cmdLineSplit[0])
            {
                Arguments = cmdLineSplit[1],
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            try
            {
                s_lock?.WaitOne();

                Program.Log($"Executing command \"{cmdLine}\"...");

                using (Process m_process = Process.Start(psi))
                {
                    m_process.OutputDataReceived += (sender, args) => { Program.Log(args.Data); };
                    m_process.ErrorDataReceived += (sender, args) => { Program.Log(args.Data, true); };
                    m_process.WaitForExit();
                }
            }
            finally
            {
                s_lock?.Release();
            }
        }

        private void UpdateTimestampsAndMoveFiles(string workingDirectory)
        {
            string[] files = Directory.GetFiles(workingDirectory);
            int fileCount = 0;

            foreach (string fileName in files)
            {
                FileInfo file = new FileInfo(fileName);

                if (file.Extension.Equals(".cfg", StringComparison.OrdinalIgnoreCase) || file.Extension.Equals(".dat", StringComparison.OrdinalIgnoreCase))
                {
                    string[] dateFromFileName = file.Name.Split(',');
                    DateTime dateTime = DateTime.ParseExact(dateFromFileName[0] + ',' + dateFromFileName[1], "yyMMdd,HHmmssfff", null);

                    if (dateTime != file.LastWriteTime)
                        File.SetLastWriteTime(file.FullName, dateTime);

                    string newFilePath = Path.Combine(m_localPath, file.Name);

                    if (!File.Exists(newFilePath))
                    {
                        file.CopyTo(newFilePath);
                        file.Delete();
                        fileCount++;
                    }
                }
            }

            if (fileCount > 0)
                Program.Log($"Successfully downloaded {fileCount} files.");
            else
                Program.Log("Failed to download any files.", true);
        }

        private BenRecord GetLastDownloadedFile()
        {
            FileInfo lastFile = new DirectoryInfo(m_localPath)
                .EnumerateFiles()
                .Where(fileInfo => fileInfo.Extension.Equals(".cfg", StringComparison.OrdinalIgnoreCase))
                .MaxBy(fileInfo => fileInfo.LastWriteTime);

            if ((object)lastFile == null)
                return new BenRecord(0, DateTime.MinValue, 0, string.Empty);

            int recordID = Convert.ToInt32(Path.GetFileNameWithoutExtension(lastFile.Name).Split(',').Last());
            DateTime lastWriteTime = lastFile.LastWriteTime;
            string fileName = lastFile.Name;

            return new BenRecord(recordID, lastWriteTime, 0, fileName);
        }

        private void ExecNotepad()
        {
            var psi = new ProcessStartInfo("C:\\Windows\\notepad.exe")
            {
                Arguments = "",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                s_lock?.WaitOne();
                using (Process p = Process.Start(psi))
                {
                    p.WaitForExit();
                }
            }
            catch(Exception ex)
            {
                Program.Log(ex.Message, true);
            }
            finally
            {
                s_lock?.Release();
            }
        }

        private string Get232Fn(DateTime myDate, string recordId, string timeStampType = "t")
        {
            DateTime dateUtc = myDate.ToUniversalTime();
            long tzoffset = Math.Abs((myDate - dateUtc).Hours) * -1;
            return myDate.ToString("yyMMdd,HHmmssfff") + "," + tzoffset + timeStampType + "," + m_siteName.Replace(" ", "_") + "," + m_serialNumber + ",TVA," + recordId;
        }

        private string GetShortPath(string longPathName)
        {
            StringBuilder shortNameBuffer = new StringBuilder(256);
            int bufferSize = GetShortPathName(longPathName, shortNameBuffer, shortNameBuffer.Capacity);
            if (bufferSize == 0) throw new System.ComponentModel.Win32Exception();
            return shortNameBuffer.ToString();
        }

        #endregion

        #region [ Static ]

        // Static Fields
        private static Semaphore s_lock;

        // Static Constructor
        static BenRunner()
        {
            int setting = ConfigurationFile.Current.Settings["systemSettings"]["BenlinkInstanceCount"]?.ValueAsInt32() ?? 0;

            if (setting > 0)
                s_lock = new Semaphore(setting, setting, "BenRunner");
        }

        // Static Methods
        [DllImport("kernel32", EntryPoint = "GetShortPathName", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetShortPathName(string longPath, StringBuilder shortPath, int bufSize);

        #endregion
    }
}
