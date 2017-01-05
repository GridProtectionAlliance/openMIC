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
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GSF;
using GSF.Identity;
using System.Security.Principal;
using GSF.Configuration;
using GSF.Data;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using FileSystem = Microsoft.VisualBasic.FileIO.FileSystem;

namespace BenDownloader
{
    public class BenRunner
    {
        #region [Members]

        private const long BENMAXFILESIZE = 7000;
        private const int MAXFILELIMIT = 100;

        private readonly string m_ipAddress;
        private readonly string m_localPath;
        private readonly string m_folder;
        private readonly string m_siteName;
        private readonly string m_serialNumber;
        private readonly string m_tempDirectoryName;
        private readonly BenRecord m_lastFileDownloaded;
        
        //private static Mutex s_mutex;
        //private readonly string m_domain;
        //private readonly string m_userName;
        //private readonly string m_passWord;
        //private readonly WindowsImpersonationContext m_context;

        #endregion

        #region [Constructors]

        public BenRunner(int deviceId, int taskId)
        {
            try
            {
                using (AdoDataConnection conn = new AdoDataConnection("systemSettings"))
                {
                    string taskSettingsString = conn.ExecuteScalar<string>("Select Settings From ConnectionProfileTask WHERE ID = {0}", taskId);
                    Dictionary<string, string> taskSettings = taskSettingsString.ParseKeyValuePairs();
                    string deviceConnectionString = conn.ExecuteScalar<string>("Select ConnectionString From Device WHERE ID = {0}", deviceId);
                    Dictionary<string, string> deviceConnection = deviceConnectionString.ParseKeyValuePairs();

                    m_folder = conn.ExecuteScalar<string>("Select OriginalSource From Device WHERE ID = {0}", deviceId);
                    m_ipAddress = deviceConnection["connectionUserName"].Split('&')[0];
                    m_localPath = taskSettings["localPath"];
                    m_siteName = conn.ExecuteScalar<string>("Select Name From Device WHERE ID = {0}", deviceId);
                    m_serialNumber = deviceConnection["connectionUserName"].Split('&')[1];

                    //m_domain = taskSettings["directoryAuthUserName"].Split('\\')[0];
                    //m_userName = taskSettings["directoryAuthUserName"].Split('\\')[1];
                    //m_passWord = taskSettings["directoryAuthPassword"];

                    string tempDirectory = System.IO.Path.GetTempPath();
                    System.IO.Directory.CreateDirectory(tempDirectory + "\\BenDownloader\\" + m_siteName);
                    m_tempDirectoryName = tempDirectory + "\\BenDownloader\\" + m_siteName;

                    //try
                    //{
                    //    GSF.IO.FilePath.DisconnectFromNetworkShare(m_localPath);
                    //}
                    //catch (Exception ex)
                    //{

                    //}

                    //s_mutex = new Mutex(false, m_serialNumber);

                    //GSF.IO.FilePath.ConnectToNetworkShare(m_localPath, m_userName, m_passWord, m_domain);
                    //m_context = GSF.Identity.UserInfo.ImpersonateUser(m_domain, m_userName, m_passWord);

                    m_lastFileDownloaded = GetLastDownloadedFile();

                }
            }
            catch(Exception ex)
            {
                Program.Log(ex.ToString());
            }
        }

        //~BenRunner()
        //{
        //    //GSF.IO.FilePath.DisconnectFromNetworkShare(m_localPath);
        //    //GSF.Identity.UserInfo.EndImpersonation(m_context);
        //    //s_mutex.Dispose();
        //}

        #endregion

        #region [Methods]
        public bool XferAllFiles()
        {
            try
            {
                XferDataFiles();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ben5K XferAllFiles (" + m_siteName + "): " + ex.ToString());
                Program.Log("Ben5K XferAllFiles (" + m_siteName + "): " + ex.ToString());
                return false;
            }
        }

        private void XferDataFiles()
        {
            List<BenRecord> myFiles = GetFileList();

            try
            {
                int numFiles = myFiles.Count;

                if (numFiles > 0)
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
                        SendTooManyFilesEmailNotification(numFiles);
                        throw new System.Exception("Site " + m_siteName + " has too many files, aborting download.");
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Log("XFER Error/Site: " + m_siteName + " - " + ex.ToString());

                throw new System.Exception("XFER Error/Site: " + m_siteName + " - " + ex.ToString());

            }
            finally
            {
                //lastFileDownloaded = curRecId.ToString();
                FileSystem.DeleteFile(m_localPath + "\\" + m_folder + "\\bendir.txt");

            }
        }

        private List<BenRecord> GetFileList()
        {
            List<BenRecord> downloadList = new List<BenRecord>();
            string dirFile = m_localPath + "\\" + m_folder + "\\bendir.txt";

            try
            {
                //delete the existing dir file if one exists.
                if (FileSystem.FileExists(dirFile))
                    FileSystem.DeleteFile(dirFile);

                //build new dir files.
                BuildBenLinkDirINI();
                
                ExecBenCommand();

                // build list of records to download
                // todo: build an algroithm for rollover of record numbers
                if (FileSystem.FileExists(dirFile))
                {
                    TextFieldParser dirReader = FileSystem.OpenTextFieldParser(dirFile, new string[] {"\t" });
                    while (!dirReader.EndOfData)
                    {
                        string[] curRow = dirReader.ReadFields();

                        if(Convert.ToInt32(curRow[0]) < m_lastFileDownloaded.Id)
                        {
                            SendFileNumberLargerEmailNotification(curRow[0], m_lastFileDownloaded.Id);
                        }

                        if (Convert.ToInt32(curRow[2]) < BENMAXFILESIZE)
                        {
                            BenRecord curRecord = new BenRecord(Convert.ToInt32(curRow[0]), Convert.ToDateTime(curRow[1]), Convert.ToInt32(curRow[2]));

                            // John Shugart wants to know that here is where we are compairing each line of the bendir.txt against the date of the
                            // last downloaded file.
                            if(curRecord.DateTime > m_lastFileDownloaded.DateTime)
                                downloadList.Add(curRecord);
                        }
                        else
                        {
                            SendFileTooLargeEmailNotification("Record id: " + curRow[0]);
                        }
                     
                    }
                    dirReader.Close();

                }
                else
                {
                    throw new Exception("GetFileList Error: " + m_siteName + " - dir file does not exist.");
                }

            }
            catch (Exception ex)
            {
                Program.Log("GetFileList Error: " + m_siteName + " - " + ex.ToString());

                throw new Exception("GetFileList Error: " + m_siteName + " - " + ex.ToString());
            }
            return downloadList;
        }

        private bool ExecBenCommand()
        {
            int exitcode = -1;

            try
            {
                string benLinCmdLine = ConfigurationFile.Current.Settings["systemSettings"]["BenLinkCommandLine"].Value;
                string cmdLine = benLinCmdLine.Replace("xxx", GetShortPath(m_localPath + "\\" + m_folder));
                string[] cmdLineSplit = cmdLine.Split(new char[] { ' ' }, 2);
                var psi = new ProcessStartInfo(cmdLineSplit[0])
                {
                    Arguments = cmdLineSplit[1],
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process p = Process.Start(psi))
                {
                    p.WaitForExit();
                    exitcode = p.ExitCode;
                }

                //bool flag = true;

                //while (flag)
                //{
                //    try
                //    {
                //        s_mutex.WaitOne();
                //        using (Process p = Process.Start(psi))
                //        {
                //            p.WaitForExit();
                //            if (p.ExitCode == 0)
                //                flag = false;
                //        }
                //    }
                //    finally
                //    {
                //        s_mutex.ReleaseMutex();
                //    }

                //}

                FileSystem.DeleteFile(m_localPath + "\\" + m_folder + "\\benlink.req");
                FileSystem.DeleteFile(m_localPath + "\\" + m_folder + "\\benlink.rsp");
                
            }
            catch (Exception ex)
            {
                Program.Log("ExecBenCommand error: " + m_siteName + " - " + ex.ToString());

                throw new Exception("ExecBenCommand error: " + m_siteName + " - " + ex.ToString());
            }

            return exitcode == 0;
        }

        private void BuildBenLinkDLINI(List<BenRecord> fileList)
        {
            string requestfilename = m_localPath + "\\" + m_folder + "\\benlink.req";

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
                                "DataDirectory=" + m_localPath + "\\" + m_folder + '\\' + System.Environment.NewLine +
                                System.Environment.NewLine +
                                "CommAddress=1" + System.Environment.NewLine +
                                "[ConnectionParam]" + System.Environment.NewLine +
                                "AccessType=0" + System.Environment.NewLine +
                                "UserName=0" + System.Environment.NewLine +
                                "IPAddress=" + m_ipAddress + System.Environment.NewLine +
                                "HangupTimeout=0";

            int i = 1;
            int curYear = DateTime.Now.Year;

            foreach (BenRecord currec in fileList)
            {
                if (currec.DateTime.Year > curYear)
                {
                    SendFileFromFutureNotification("Record ID: " + currec.Id);
                    throw new System.Exception("FileID " + currec.Id + " at site " + m_siteName + " from the future. Fix DFR clock.");
                }
                if (currec.Id < m_lastFileDownloaded.Id)
                {
                    SendFileNumberLargerEmailNotification("Record ID: " + currec.Id.ToString(), currec.Id);
                    throw new System.Exception("FileID " + currec.Id + " at site " + m_siteName + " Id less than last downloaded.");
                }


                myINIFile += System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + "[Request" + i++ + "]" + System.Environment.NewLine +
                            "RequestType=2" + System.Environment.NewLine +
                            "RecordNum=" + currec.Id + System.Environment.NewLine +
                            "SubBenNum=0" + System.Environment.NewLine +
                            "Origin=1" + System.Environment.NewLine +
                            "OptionFlags=1" + System.Environment.NewLine +
                            "DataPath=" + m_tempDirectoryName + System.Environment.NewLine +
                            "FileName=" + Get232Fn(currec.DateTime, currec.Id.ToString());
                
            }


            try
            {
                System.IO.FileInfo file = new System.IO.FileInfo(requestfilename);
                file.Directory.Create();
                FileSystem.WriteAllText(file.FullName, myINIFile, false, System.Text.Encoding.ASCII);
            }
            catch (Exception ex)
            {
                Program.Log("BuildBenLinkDLINI error: " + m_siteName + " - " + ex.ToString());

                throw new System.Exception("BuildBenLinkDLINI error: " + m_siteName + " - " + ex.ToString());
            }
        }

        private void BuildBenLinkDirINI()
        {
            string requestFileName = m_localPath + "\\" + m_folder + "\\benlink.req";

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
                                "DataDirectory=" + m_localPath + "\\" + m_folder + '\\' + System.Environment.NewLine +
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
                                "DataPath=" + m_localPath + "\\" + m_folder;

            try
            {
                System.IO.FileInfo file = new System.IO.FileInfo(requestFileName);
                file.Directory.Create();
                FileSystem.WriteAllText(file.FullName, myINIFile, false, System.Text.Encoding.ASCII);
            }
            catch (Exception ex)
            {
                Program.Log("BuildBenLinkDirINI error: " + m_siteName + '-' + ex.ToString());

                throw new SystemException("BuildBenLinkDirINI error: " + m_siteName + '-' + ex.ToString());
            }

        }

        private void UpdateTimestamps()
        {

            string[] files = System.IO.Directory.GetFiles(m_tempDirectoryName);

            foreach (string fileName in files)
            {
                System.IO.FileInfo file = new System.IO.FileInfo(fileName);
                if(file.Name.EndsWith("cfg") || file.Name.EndsWith("dat"))
                {
                    try
                    {
                        string[] dateFromFileName = file.Name.Split(',');
                        DateTime dateTime = DateTime.ParseExact(dateFromFileName[0] + ',' + dateFromFileName[1], "yyMMdd,HHmmssfff", null);
                        if(dateTime != file.LastWriteTime)
                        {
                            System.IO.File.SetLastWriteTime(file.FullName, dateTime);
                            string newFileName = m_localPath + "\\" + m_folder + '\\' + file.Name;
                            System.IO.File.Copy(file.FullName, newFileName);
                            System.IO.File.Delete(file.FullName);
                        }
                    }
                    catch(Exception ex)
                    {
                        Program.Log("File timestamp update error: " + file.Name + '-' + ex.ToString());

                        throw new SystemException("File timestamp update error: " + file.Name + '-' + ex.ToString());
                    }
                }
            }
            System.IO.Directory.Delete(m_tempDirectoryName);
        }

        private BenRecord GetLastDownloadedFile()
        {
            string[] files;
            BenRecord lastFile = new BenRecord(0, DateTime.MinValue, 0);

            try
            {
                files = System.IO.Directory.GetFiles(m_localPath + "\\" + m_folder);

            foreach (string fileName in files)
            {
                System.IO.FileInfo file = new System.IO.FileInfo(fileName);
                if (file.Name.EndsWith("cfg") || file.Name.EndsWith("dat"))
                {
                    try
                    {
                    if (file.LastWriteTime > lastFile.DateTime)
                        {
                            string[] dateFromFileName = System.IO.Path.GetFileNameWithoutExtension(file.Name).Split(',');
                            lastFile.DateTime = DateTime.ParseExact(dateFromFileName[0] + ',' + dateFromFileName[1], "yyMMdd,HHmmssfff", null);
                            lastFile.Id = int.Parse(dateFromFileName[dateFromFileName.Length - 1]);
                        }
                    }
                    catch (Exception ex)
                    {
                        Program.Log("Retrieving Last Downloaded File error: " + file.Name + '-' + ex.ToString());

                        throw new SystemException("Retrieving Last Downloaded File error: " + file.Name + '-' + ex.ToString());
                    }
                }
            }
            }
            catch (Exception ex)
            {
                Program.Log("Get Last Downloaded File - Get Files \n\n" + ex.ToString());
            }


            return lastFile;
        }


        #endregion

        #region [File System]
        private string Get232Fn(DateTime myDate, string recordId, string timeStampType = "t")
        {
            DateTime dateUtc = myDate.ToUniversalTime();
            long tzoffset = Math.Abs(DateAndTime.DateDiff(DateInterval.Hour, myDate, dateUtc)) * -1;
            return myDate.ToString("yyMMdd,HHmmssfff") + "," + tzoffset + timeStampType + "," + m_siteName.Replace(" ", "_") + "," + m_serialNumber + ",TVA," + recordId;
        }

        private string GetShortPath(string longPathName)
        {
            StringBuilder shortNameBuffer = new StringBuilder(256);
            int bufferSize = GetShortPathName(longPathName, shortNameBuffer, shortNameBuffer.Capacity);
            if (bufferSize == 0) throw new System.ComponentModel.Win32Exception();
            return shortNameBuffer.ToString();
        }

        [DllImport("kernel32", EntryPoint = "GetShortPathName", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetShortPathName(string longPath, StringBuilder shortPath, int bufSize);
        #endregion

        #region [Email/PQMS]
        private void SendFileNumberLargerEmailNotification(string filename, int downloadFileNumber)
        {
            string msgBody = "Largest file on DFR is " + filename + ". The largest file already downloaded is " + downloadFileNumber + ".  " +
                             "This site may need to be checked.  It appears files were deleted from the DFR.  The counter on the Downloader should be reset.";
            SendEmailDefault(msgBody);
        }

        private void SendFileTooLargeEmailNotification(string filename)
        {
            string msgBody = "File " + filename + " too large to download (greater than 4 MB).  Please check site...";
            SendEmailDefault(msgBody);
        }

        private void SendFileFromFutureNotification(string filename)
        {
            string msgBody = "File " + filename + " has a timestamp from the future, please check the DFR clock.";
            SendEmailDefault(msgBody);
        }

        private void SendTooManyFilesEmailNotification(int fileCount)
        {
            string msgBody = "Site has " + fileCount + " files to download.  This site may be a candidate for chip failure.  Please check...";
            SendEmailDefault(msgBody);
        }

        private void SendExceptionEmail(string exception)
        {
            SendEmail("tllaughner@tva.gov", "bendownloader@gpa.org", "Exception from bendownloader", exception);
        }


        private void SendEmailDefault(string msgBody)
        {
#if DEBUG
            string msgTo = "tllaughner@tva.gov";
#else
            string msgTo = "powerquality@tva.gov";
#endif
            string msgFrom = "powerquality@tva.gov";
            string msgSubject = "Site " + m_siteName + " problem";

            SendEmail(msgTo, msgFrom, msgSubject, msgBody);
        }

        private void SendEmail(string msgTo, string msgFrom, string msgSubject, string msgBody)
        {
            if (bool.Parse(ConfigurationFile.Current.Settings["systemSettings"]["EmailNotificationsEnabled"].Value))
            {
                MailAddress mf = new MailAddress(msgFrom);
                MailAddress mt = new MailAddress(msgTo);
                MailMessage msg = new MailMessage(mf, mt);
                msg.Subject = msgSubject;
                msg.Body = msgBody;

                SmtpClient mclient = new SmtpClient(ConfigurationFile.Current.Settings["systemSettings"]["Mailserver"].Value);
                mclient.Send(msg);
            }
        }


        #endregion

    }

}
