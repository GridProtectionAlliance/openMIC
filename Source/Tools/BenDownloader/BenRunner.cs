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
using System.Threading;

namespace BenDownloader
{
    public class BenRunner
    {
        #region [Members]
        private const long BENMAXFILESIZE = 7000;
        private const int MAXFILELIMIT = 100;

        private readonly string m_ipAddress;
        private readonly string m_localPath;
        private readonly string m_siteName;
        private readonly string m_serialNumber;
        private readonly string m_tempDirectoryName;
        private readonly BenRecord m_lastFileDownloaded;
        private string m_lastFileDownloadedThisSession;
        #endregion

        #region [Constructors]

        public BenRunner(string localPath, string siteName, string ip, string serialNumber)
        {
            m_localPath = localPath;
            m_siteName = siteName;
            m_ipAddress = ip;
            m_serialNumber = serialNumber;

            try
            {
                string tempDirectory = Path.GetTempPath();
                Directory.CreateDirectory(tempDirectory + "\\BenDownloader\\" + m_siteName);
                m_tempDirectoryName = tempDirectory + "BenDownloader\\" + m_siteName + "\\";
            }
            catch(Exception ex)
            {
                Program.Log("Failed to initialize BenRunner: " + ex.Message);
            }

        }

        #endregion

        #region [ Static ]
        private static Semaphore s_lock;

        static BenRunner()
        {
            int setting = Program.OpenMiConfigurationFile.Settings["systemSettings"]["BenRunnerInstanceCount"]?.ValueAsInt32() ?? 0;

            if (setting > 0)
                s_lock = new Semaphore(setting, setting, "BenRunner");
        }

        #endregion

        #region [Methods]

        public bool XferAllFiles()
        {
            try
            {
                XferDataFiles();
                //ExecNotepad();
                return true;
            }
            catch (Exception ex)
            {
                Program.Log("Ben5K XferAllFiles (" + m_siteName + "): " + ex.ToString(), true);
                return false;
            }
        }

        private void XferDataFiles()
        {
            List<BenRecord> myFiles = GetFileList();

            try
            {
                int numFiles = myFiles.Count;

                if (myFiles.Any())
                {
                    //if there are less than 25 records to download then begin downloading files
                    if (numFiles + 50 < MAXFILELIMIT)
                    {
                        BuildBenLinkDLINI(myFiles);
                        ExecBenCommand();
                        UpdateTimestamps();
                    }
                    else
                    {
                        throw new System.Exception("Site " + m_siteName + " has too many files, aborting download.");
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Log("XFER Error/Site: " + m_siteName + " - " + ex.ToString(), true);

                throw new System.Exception("XFER Error/Site: " + m_siteName + " - " + ex.ToString());

            }
            finally
            {
                File.Delete(m_tempDirectoryName + "bendir.txt");

            }
        }

        private List<BenRecord> GetFileList()
        {
            List<BenRecord> downloadList = new List<BenRecord>();
            string dirFile = m_tempDirectoryName + "bendir.txt";

            try
            {
                //delete the existing dir file if one exists.
                if (File.Exists(dirFile))
                    File.Delete(dirFile);

                //build new dir files.
                BuildBenLinkDirINI();
                
                ExecBenCommand();

                // build list of records to download
                if (File.Exists(dirFile))
                {
                    TextReader dirReader = File.OpenText(dirFile);
                    string line;
                    while ((line = dirReader.ReadLine()) != null)
                    {
                        string[] curRow = line.Split('\t');

                        if (Convert.ToInt32(curRow[2]) < BENMAXFILESIZE)
                        {
                            BenRecord curRecord = new BenRecord(Convert.ToInt32(curRow[0]), Convert.ToDateTime(curRow[1]), Convert.ToInt32(curRow[2]), Get232Fn(Convert.ToDateTime(curRow[1]), curRow[0]));

                            if(curRecord.DateTime > DateTime.UtcNow.AddDays(-30) && curRecord.DateTime > m_lastFileDownloaded.DateTime && !File.Exists(Path.Combine(m_localPath,curRecord.Name)))
                                downloadList.Add(curRecord);
                        }
                        else
                            Program.Log("File too large Error: " + m_siteName + " - " + Convert.ToString(curRow[0]) , true);
                    }
                    dirReader.Close();

                }
                else
                    throw new Exception("GetFileList Error: " + m_siteName + " - dir file does not exist.");


            }
            catch (Exception ex)
            {
                Program.Log("GetFileList Error: " + m_siteName + " - " + ex.ToString(), true);
                throw new Exception("GetFileList Error: " + m_siteName + " - " + ex.ToString());
            }
            return downloadList;
        }

        private bool ExecBenCommand()
        {
            int exitcode = -1;

            try
            {
                string benLinCmdLine = Program.OpenMiConfigurationFile.Settings["systemSettings"]["BenLinkCommandLine"].Value;
                string cmdLine = benLinCmdLine.Replace("xxx", "\"" + m_tempDirectoryName + "\"");
                string[] cmdLineSplit = cmdLine.Split(new char[] { ' ' }, 2);
                var psi = new ProcessStartInfo(cmdLineSplit[0])
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
                    using (Process m_process = Process.Start(psi))
                    {
                        m_process.OutputDataReceived += (sender, args) => { Program.Log(args.Data); };
                        m_process.ErrorDataReceived += (sender, args) => { Program.Log(args.Data, true); };
                        m_process.WaitForExit();
                        exitcode = m_process.ExitCode;
                    }
                }
                catch(Exception ex)
                {
                    Program.Log("BenLink failed to run: " + ex.Message, true);
                }
                finally
                {
                    s_lock?.Release();
                }

                File.Delete(m_tempDirectoryName + "benlink.req");
                if(File.Exists(m_tempDirectoryName + "benlink.rsp"))
                    File.Delete(m_tempDirectoryName + "benlink.rsp");
                
            }
            catch (Exception ex)
            {
                Program.Log("ExecBenCommand error: " + m_siteName + " - " + ex.ToString(), true);

                throw new Exception("ExecBenCommand error: " + m_siteName + " - " + ex.ToString());
            }

            return exitcode == 0;
        }

        private void BuildBenLinkDLINI(List<BenRecord> fileList)
        {
            string requestfilename = m_tempDirectoryName + "benlink.req";

            string myINIFile = "[Signature]" + System.Environment.NewLine +
                               "Program=BenLink" + System.Environment.NewLine +
                               "FileType=Request" + System.Environment.NewLine +
                               "FileVersion=1" + System.Environment.NewLine +
                                System.Environment.NewLine +
                                "[General]" + System.Environment.NewLine +
                                "NbRequest=" + fileList.Count.ToString() + System.Environment.NewLine +
                                System.Environment.NewLine +
                                "[Device]" + System.Environment.NewLine +
                                "DeviceType=5" + System.Environment.NewLine +
                                "DeviceSN=" + m_serialNumber + System.Environment.NewLine +
                                "NominalFrequency=60" + System.Environment.NewLine +
                                "DataDirectory=" + m_tempDirectoryName + System.Environment.NewLine +
                                System.Environment.NewLine +
                                "CommAddress=1" + System.Environment.NewLine +
                                "[ConnectionParam]" + System.Environment.NewLine +
                                "AccessType=0" + System.Environment.NewLine +
                                "UserName=0" + System.Environment.NewLine +
                                "IPAddress=" + m_ipAddress + System.Environment.NewLine +
                                "HangupTimeout=0";

            int i = 1;

            Program.Log("StartFileList");
            foreach (BenRecord currec in fileList)
            {
                myINIFile += System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + "[Request" + i++ + "]" + System.Environment.NewLine +
                            "RequestType=2" + System.Environment.NewLine +
                            "RecordNum=" + currec.Id + System.Environment.NewLine +
                            "SubBenNum=0" + System.Environment.NewLine +
                            "Origin=1" + System.Environment.NewLine +
                            "OptionFlags=2" + System.Environment.NewLine +
                            "DataPath=" + m_tempDirectoryName + System.Environment.NewLine +
                            "FileName=" + currec.Name;

                Program.Log("FileList:" + currec.Name);
            }
            Program.Log("EndFileList");

            try
            {
                System.IO.FileInfo file = new System.IO.FileInfo(requestfilename);
                file.Directory.Create();
                File.WriteAllText(file.FullName, myINIFile,System.Text.Encoding.ASCII);
            }
            catch (Exception ex)
            {
                Program.Log("BuildBenLinkDLINI error: " + m_siteName + " - " + ex.ToString(), true);

                throw new System.Exception("BuildBenLinkDLINI error: " + m_siteName + " - " + ex.ToString());
            }
        }

        private void BuildBenLinkDirINI()
        {
            string requestFileName = m_tempDirectoryName + "benlink.req";

            string myINIFile = "[Signature]" + System.Environment.NewLine +
                               "Program=BenLink" + System.Environment.NewLine +
                               "FileType=Request" + System.Environment.NewLine +
                               "FileVersion=1" + System.Environment.NewLine +
                                System.Environment.NewLine +
                                "[General]" + System.Environment.NewLine +
                                "NbRequest=1" + System.Environment.NewLine +
                                System.Environment.NewLine +
                                "[Device]" + System.Environment.NewLine +
                                "DeviceType=5" + System.Environment.NewLine +
                                "DeviceSN=" + m_serialNumber + System.Environment.NewLine +
                                "NominalFrequency=60" + System.Environment.NewLine +
                                "DataDirectory=" + m_tempDirectoryName + System.Environment.NewLine +
                                System.Environment.NewLine +
                                "[ConnectionParam]" + System.Environment.NewLine +
                                "AccessType=0" + System.Environment.NewLine +
                                "UserName=0" + System.Environment.NewLine +
                                "CommAddress=1" + System.Environment.NewLine +
                                "IPAddress=" + m_ipAddress + System.Environment.NewLine +
                                "HangupTimeout=0" + System.Environment.NewLine +
                                System.Environment.NewLine +
                                "[Request1]" + System.Environment.NewLine +
                                "RequestType=1" + System.Environment.NewLine +
                                "Origin=1" + System.Environment.NewLine +
                                "SubBens=1" + System.Environment.NewLine +
                                "DataPath=" + m_tempDirectoryName;

            try
            {
                System.IO.FileInfo file = new System.IO.FileInfo(requestFileName);
                file.Directory.Create();
                File.WriteAllText(file.FullName, myINIFile, System.Text.Encoding.ASCII);
            }
            catch (Exception ex)
            {
                Program.Log("BuildBenLinkDirINI error: " + m_siteName + '-' + ex.ToString(), true);

                throw new SystemException("BuildBenLinkDirINI error: " + m_siteName + '-' + ex.ToString());
            }

        }

        private void UpdateTimestamps()
        {
            string[] files = System.IO.Directory.GetFiles(m_tempDirectoryName);

            foreach (string fileName in files)
            {
                System.IO.FileInfo file = new System.IO.FileInfo(fileName);
                if(file.Name.EndsWith("cfg") || file.Name.EndsWith("dat") )
                {
                    try
                    {
                        string[] dateFromFileName = file.Name.Split(',');
                        DateTime dateTime = DateTime.ParseExact(dateFromFileName[0] + ',' + dateFromFileName[1], "yyMMdd,HHmmssfff", null);
                        if(dateTime != file.LastWriteTime)
                            System.IO.File.SetLastWriteTime(file.FullName, dateTime);

                        string newFileName = m_localPath + file.Name;
                        System.IO.FileInfo fi = new System.IO.FileInfo(newFileName);
                        if (!fi.Exists)
                        {
                            m_lastFileDownloadedThisSession = file.Name;
                            System.IO.File.Copy(file.FullName, newFileName);
                        }
                        
                    }
                    catch(Exception ex)
                    {
                        Program.Log("File timestamp update error: " + file.Name + '-' + ex.ToString(), true);
                        throw new SystemException("File timestamp update error: " + file.Name + '-' + ex.ToString());
                    }
                }

                if(file.Name != "BenDownloaderLogFile.txt")
                    System.IO.File.Delete(file.FullName);

            }
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

        #endregion

        #region [File System]
        private string Get232Fn(DateTime myDate, string recordId, string timeStampType = "t")
        {
            DateTime dateUtc = myDate.ToUniversalTime();
            long tzoffset = Math.Abs((myDate - dateUtc).Hours) * -1;
            return myDate.ToString("yyMMdd,HHmmssfff") + "," + tzoffset + timeStampType + "," + m_siteName.Replace(" ", "_") + "," + m_serialNumber + ",TVA," + recordId;
        }
        #endregion

    }

}
