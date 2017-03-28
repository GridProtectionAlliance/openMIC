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
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using GSF;
using GSF.Data;
using GSF.IO;
using GSF.Parsing;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using FileSystem = Microsoft.VisualBasic.FileIO.FileSystem;
namespace BenDownloader
{
    public class BenRunner : IDisposable
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
        private readonly DataRow m_deviceRecord;
        private readonly DataRow m_connectionProfile;
        private readonly Dictionary<string, string> m_connectionProfileTaskSettings;
        private string m_lastFileDownloadedThisSession;
        private readonly AdoDataConnection m_adoDataConnection;
        private readonly SafeFileWatcher m_fileWatcher;
        private long m_lastFileChangedTime;
        private Process m_process;
        private readonly System.Timers.Timer m_activityMonitor;
        private bool m_disposed;
        private static bool s_emailSentTooManyFiles;
        private static bool s_emailSentFileNumberLarger;
        private static bool s_emailSentFileTooLarge;
        private static bool s_emailSentFileFromFuture;

        #endregion

        #region [Constructors]

        public BenRunner(int deviceId, int taskId)
        {
            m_adoDataConnection = new AdoDataConnection(Program.OpenMiConfigurationFile.Settings["systemSettings"]["ConnectionString"].Value, Program.OpenMiConfigurationFile.Settings["systemSettings"]["DataProviderString"].Value);

            try
            {

                string taskSettingsString = m_adoDataConnection.ExecuteScalar<string>("Select Settings From ConnectionProfileTask WHERE ID = {0}", taskId);
                m_connectionProfileTaskSettings = taskSettingsString.ParseKeyValuePairs();
                m_deviceRecord = m_adoDataConnection.RetrieveRow("Select * From Device WHERE ID = {0}", deviceId);
                Dictionary<string, string> deviceConnection = m_deviceRecord["connectionString"].ToString().ParseKeyValuePairs();

                m_connectionProfile = m_adoDataConnection.RetrieveRow("SELECT * FROM connectionProfile WHERE ID = {0}", deviceConnection["connectionProfileID"]);
                m_folder = m_deviceRecord["OriginalSource"].ToString();
                m_ipAddress = deviceConnection["connectionUserName"].Split('&')[0];
                m_localPath = m_connectionProfileTaskSettings["localPath"];
                m_siteName = m_deviceRecord["Name"].ToString();
                m_serialNumber = deviceConnection["connectionUserName"].Split('&')[1];

                string tempDirectory = System.IO.Path.GetTempPath();
                System.IO.Directory.CreateDirectory(tempDirectory + "\\BenDownloader\\" + m_siteName);
                m_tempDirectoryName = tempDirectory + "BenDownloader\\" + m_siteName + "\\";
                //Console.WriteLine(m_tempDirectoryName);

                m_fileWatcher = new SafeFileWatcher(m_tempDirectoryName);
                m_fileWatcher.NotifyFilter = NotifyFilters.Size | NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.FileName;
                m_fileWatcher.Changed += m_fileWatcher_Changed;
                m_fileWatcher.Created += m_fileWatcher_Changed;

                m_activityMonitor = new System.Timers.Timer(5000.0D);
                m_activityMonitor.Elapsed += m_activityMonitor_Elapsed;
                m_activityMonitor.AutoReset = true;

                m_lastFileDownloaded = GetLastDownloadedFile();
                m_lastFileDownloadedThisSession = "";

            }
            catch (Exception ex)
            {
                Program.Log(ex.ToString(), m_tempDirectoryName);
            }
        }

        ~BenRunner()
        {
            Dispose();
        }

        #endregion

        #region [Methods]

        /// <summary>
        /// Releases all the resources used by the <see cref="BenRunner"/> object.
        /// </summary>
        public void Dispose()
        {
            if (!m_disposed)
            {
                m_disposed = true;

                m_process?.Kill();
                m_adoDataConnection?.Dispose();
                m_fileWatcher?.Dispose();
                m_activityMonitor?.Dispose();

                GC.SuppressFinalize(this);
            }
        }

        public bool XferAllFiles()
        {
            try
            {
                XferDataFiles();
                //UpdateTimestamps();
                //ExecNotepad();

                UpdateStatusLogDatabase("", true);
                return true;
            }
            catch (Exception ex)
            {
                UpdateStatusLogDatabase(ex.Message, false);
                Program.Log("Ben5K XferAllFiles (" + m_siteName + "): " + ex.ToString(), m_tempDirectoryName);
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

                        m_fileWatcher.EnableRaisingEvents = true;
                        ExecBenCommand();
                        m_fileWatcher.EnableRaisingEvents = false;

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
                Program.Log("XFER Error/Site: " + m_siteName + " - " + ex.ToString(),m_tempDirectoryName);

                throw new System.Exception("XFER Error/Site: " + m_siteName + " - " + ex.ToString());

            }
            finally
            {
                FileSystem.DeleteFile(m_tempDirectoryName + "bendir.txt");

            }
        }

        private List<BenRecord> GetFileList()
        {
            List<BenRecord> downloadList = new List<BenRecord>();
            string dirFile = m_tempDirectoryName + "bendir.txt";

            try
            {
                //delete the existing dir file if one exists.
                if (FileSystem.FileExists(dirFile))
                    FileSystem.DeleteFile(dirFile);

                //build new dir files.
                BuildBenLinkDirINI();
                
                ExecBenCommand();

                // build list of records to download
                if (FileSystem.FileExists(dirFile))
                {
                    TextFieldParser dirReader = FileSystem.OpenTextFieldParser(dirFile, new string[] {"\t" });
                    while (!dirReader.EndOfData)
                    {
                        string[] curRow = dirReader.ReadFields();

                        if(Convert.ToInt32(curRow[0]) < m_lastFileDownloaded.Id)
                            SendFileNumberLargerEmailNotification(curRow[0], m_lastFileDownloaded.Id);

                        if (Convert.ToInt32(curRow[2]) < BENMAXFILESIZE)
                        {
                            BenRecord curRecord = new BenRecord(Convert.ToInt32(curRow[0]), Convert.ToDateTime(curRow[1]), Convert.ToInt32(curRow[2]));

                            if(curRecord.DateTime > DateTime.UtcNow.AddDays(-30) && curRecord.DateTime > m_lastFileDownloaded.DateTime)
                                downloadList.Add(curRecord);
                        }
                        else
                            SendFileTooLargeEmailNotification("Record id: " + curRow[0]);
                    }
                    dirReader.Close();

                }
                else
                    throw new Exception("GetFileList Error: " + m_siteName + " - dir file does not exist.");


            }
            catch (Exception ex)
            {
                Program.Log("GetFileList Error: " + m_siteName + " - " + ex.ToString(), m_tempDirectoryName);
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
                string cmdLine = benLinCmdLine.Replace("xxx", GetShortPath(m_tempDirectoryName));
                string[] cmdLineSplit = cmdLine.Split(new char[] { ' ' }, 2);
                var psi = new ProcessStartInfo(cmdLineSplit[0])
                {
                    Arguments = cmdLineSplit[1],
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                if (m_fileWatcher.EnableRaisingEvents)
                {
                    m_lastFileChangedTime = DateTime.UtcNow.Ticks;
                    m_activityMonitor.Enabled = true;
                }

                // ReSharper disable once PossibleNullReferenceException
                using (m_process = Process.Start(psi))
                {
                    m_process.WaitForExit();
                    exitcode = m_process.ExitCode;
                }

                FileSystem.DeleteFile(m_tempDirectoryName + "benlink.req");
                if(File.Exists(m_tempDirectoryName + "benlink.rsp"))
                    FileSystem.DeleteFile(m_tempDirectoryName + "benlink.rsp");
                
            }
            catch (Exception ex)
            {
                Program.Log("ExecBenCommand error: " + m_siteName + " - " + ex.ToString(), m_tempDirectoryName);

                throw new Exception("ExecBenCommand error: " + m_siteName + " - " + ex.ToString());
            }
            finally
            {
                m_activityMonitor.Enabled = false;
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
                            "OptionFlags=2" + System.Environment.NewLine +
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
                Program.Log("BuildBenLinkDLINI error: " + m_siteName + " - " + ex.ToString(), m_tempDirectoryName);

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
                FileSystem.WriteAllText(file.FullName, myINIFile, false, System.Text.Encoding.ASCII);
            }
            catch (Exception ex)
            {
                Program.Log("BuildBenLinkDirINI error: " + m_siteName + '-' + ex.ToString(), m_tempDirectoryName);

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

                        string newFileName = GetLocalFileName(file.Name);
                        System.IO.FileInfo fi = new System.IO.FileInfo(newFileName);
                        if (!fi.Exists)
                        {
                            m_lastFileDownloadedThisSession = file.Name;
                            System.IO.File.Copy(file.FullName, newFileName);
                        }
                        
                    }
                    catch(Exception ex)
                    {
                        Program.Log("File timestamp update error: " + file.Name + '-' + ex.ToString(), m_tempDirectoryName);
                        throw new SystemException("File timestamp update error: " + file.Name + '-' + ex.ToString());
                    }
                }

                if(file.Name != "BenDownloaderLogFile.txt")
                    System.IO.File.Delete(file.FullName);

            }
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
                    if (file.Name.EndsWith("cfg"))
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
                            Program.Log("Retrieving Last Downloaded File error: " + file.Name + '-' + ex.ToString(), m_tempDirectoryName);
                            throw new SystemException("Retrieving Last Downloaded File error: " + file.Name + '-' + ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Log("Get Last Downloaded File - Get Files \n\n" + ex.ToString(), m_tempDirectoryName);
            }


            return lastFile;
        }

        private void ExecNotepad()
        {
            var psi = new ProcessStartInfo("C:\\Windows\\notepad.exe")
            {
                Arguments = "",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process p = Process.Start(psi))
            {
                p.WaitForExit();
            }

        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void UpdateStatusLogDatabase(string message, bool success)
        {
            try
            {
                DataRow log = m_adoDataConnection.RetrieveRow("SELECT * FROM StatusLog  WITH (NOLOCK) WHERE DeviceID = {0}", int.Parse(m_deviceRecord["ID"].ToString()));

                if (success)
                {
                    if (m_deviceRecord["ID"].ToString() == log["DeviceID"].ToString() && m_lastFileDownloadedThisSession != "")
                        m_adoDataConnection.ExecuteNonQuery("Update StatusLog SET LastSuccess = {0}, LastFile = {1} WHERE DeviceID = {2}", DateTime.UtcNow, m_lastFileDownloadedThisSession, m_deviceRecord["ID"]);
                    else if (m_deviceRecord["ID"].ToString() == log["DeviceID"].ToString() && m_lastFileDownloadedThisSession == "")
                        m_adoDataConnection.ExecuteNonQuery("Update StatusLog SET LastSuccess = {0} WHERE DeviceID = {1}", DateTime.UtcNow, m_deviceRecord["ID"]);
                    else
                        m_adoDataConnection.ExecuteNonQuery("INSERT INTO StatusLog (LastSuccess , LastFile, DeviceID) VALUES ({0},{1},{2})", DateTime.UtcNow, m_lastFileDownloadedThisSession, m_deviceRecord["ID"]);
                }
                else
                {

                    if (m_deviceRecord["ID"].ToString() == log["DeviceID"].ToString())
                        m_adoDataConnection.ExecuteNonQuery("Update StatusLog SET LastFailure = {0}, Message = {1} WHERE DeviceID = {2}", DateTime.UtcNow, message, m_deviceRecord["ID"]);
                    else
                        m_adoDataConnection.ExecuteNonQuery("INSERT INTO StatusLog (LastFailure , Message, DeviceID) VALUES ({0},{1},{2})", DateTime.UtcNow, message, m_deviceRecord["ID"]);
                }
            }
            catch (Exception ex)
            {
                Program.Log("StatusLog Update (" + m_siteName + ") failed: " + ex.Message, m_tempDirectoryName);
            }
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

        private string GetLocalFileName( string fileName)
        {
            TemplatedExpressionParser directoryNameExpressionParser = new TemplatedExpressionParser('<', '>', '[', ']');
            Dictionary<string, string> substitutions = new Dictionary<string, string>
            {
                { "<YYYY>", $"{DateTime.Now.Year}" },
                { "<YY>", $"{DateTime.Now.Year.ToString().Substring(2)}" },
                { "<MM>", $"{DateTime.Now.Month.ToString().PadLeft(2, '0')}" },
                { "<DD>", $"{DateTime.Now.Day.ToString().PadLeft(2, '0')}" },
                { "<DeviceName>", (m_deviceRecord["Name"].ToString() ==""? "undefined": m_deviceRecord["Name"].ToString())},
                { "<DeviceAcronym>", m_deviceRecord["Acronym"].ToString() },
                { "<DeviceFolderName>", (m_deviceRecord["OriginalSource"].ToString() =="" ? m_deviceRecord["Acronym"].ToString() : m_deviceRecord["OriginalSource"].ToString())},
                { "<ProfileName>", (m_connectionProfile["Name"].ToString() == "" ? "undefined" :m_connectionProfile["Name"].ToString()) }
            };

            directoryNameExpressionParser.TemplatedExpression = m_connectionProfileTaskSettings["directoryNamingExpression"].Replace("\\", "\\\\");

            //         Possible UNC Path                            Sub Directory - duplicate path slashes are removed
            fileName = m_localPath + $"{Path.DirectorySeparatorChar}{directoryNameExpressionParser.Execute(substitutions)}{Path.DirectorySeparatorChar}{fileName}".RemoveDuplicates(Path.DirectorySeparatorChar.ToString());
            //Console.WriteLine(fileName);

            string directoryName = FilePath.GetDirectoryName(fileName);

            if (!Directory.Exists(directoryName))
            {
                try
                {
                    Directory.CreateDirectory(directoryName);
                }
                catch (Exception ex)
                {
                    Program.Log($"Failed to create directory \"{directoryName}\": {ex.Message}", m_tempDirectoryName);
                    throw new InvalidOperationException($"Failed to create directory \"{directoryName}\": {ex.Message}", ex);

                }
            }

            return fileName;
        }

        private void m_fileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            m_lastFileChangedTime = DateTime.UtcNow.Ticks;
        }

        private void m_activityMonitor_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (DateTime.UtcNow.Ticks - m_lastFileChangedTime > Ticks.FromSeconds(60.0D))
            {
                m_activityMonitor.Enabled = false;

                try
                {
                    m_process?.Kill();
                }
                catch (Exception ex)
                {
                    Program.Log("Error while trying to kill no responsive downloader: " + ex.Message);
                    throw;
                }
            }
        }

        #endregion

        #region [Email/PQMS]

        private void SendFileNumberLargerEmailNotification(string filename, int downloadFileNumber)
        {
            string msgBody = "Largest file on DFR is " + filename + ". The largest file already downloaded is " + downloadFileNumber + ".  " +
                             "This site may need to be checked.  It appears files were deleted from the DFR.  The counter on the Downloader should be reset.";

            if(!s_emailSentFileNumberLarger)
                SendEmailDefault(msgBody);
            s_emailSentFileNumberLarger = true;
        }

        private void SendFileTooLargeEmailNotification(string filename)
        {
            string msgBody = "File " + filename + " too large to download (greater than 4 MB).  Please check site...";

            if (!s_emailSentFileTooLarge)
                SendEmailDefault(msgBody);
            s_emailSentFileTooLarge = true;
        }

        private void SendFileFromFutureNotification(string filename)
        {
            string msgBody = "File " + filename + " has a timestamp from the future, please check the DFR clock.";

            if (!s_emailSentFileFromFuture)
                SendEmailDefault(msgBody);
            s_emailSentFileFromFuture = true;
        }

        private void SendTooManyFilesEmailNotification(int fileCount)
        {
            string msgBody = "Site has " + fileCount + " files to download.  This site may be a candidate for chip failure.  Please check...";

            if (!s_emailSentTooManyFiles)
                SendEmailDefault(msgBody);
            s_emailSentTooManyFiles = true;

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
            try
            {
                if (bool.Parse(Program.OpenMiConfigurationFile.Settings["systemSettings"]["EmailNotificationsEnabled"]?.Value ?? "false"))
                {
                    MailAddress mf = new MailAddress(msgFrom);
                    MailAddress mt = new MailAddress(msgTo);
                    MailMessage msg = new MailMessage(mf, mt);
                    msg.Subject = msgSubject;
                    msg.Body = msgBody;

                    SmtpClient mclient = new SmtpClient(Program.OpenMiConfigurationFile.Settings["systemSettings"]["Mailserver"].Value);
                    mclient.Send(msg);
                }
            }
            catch (Exception ex)
            {
                Program.Log("Failed Sending Email: " + ex.Message);
            }
        }


        #endregion

    }

}
