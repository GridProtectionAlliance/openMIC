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
using System.Threading.Tasks;
using GSF;
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
        private const long MAXFILESIZE = 4000000;

        private string ipAddress;
        private string localPath;
        private string siteName;
        private int siteID;
        private string serialNumber;
        private string lastFileDownloaded;
        private int downloadFileNumber;
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
                    string folder = conn.ExecuteScalar<string>("Select OriginalSource From Device WHERE ID = {0}", deviceId);

                    ipAddress = deviceConnection["connectionUserName"].Split('&')[0];
                    localPath = taskSettings["localPath"]+ '\\' + folder;
                    siteName = conn.ExecuteScalar<string>("Select Name From Device WHERE ID = {0}", deviceId);
                    siteID = conn.ExecuteScalar<int>("Select ID From Device WHERE ID = {0}", deviceId);
                    serialNumber = deviceConnection["connectionUserName"].Split('&')[1];
                }
            }
            catch(Exception ex)
            {
                Program.Log(ex.ToString());
            }
            // look through current diretory to get last file downloaded name and number

        }
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
                Console.WriteLine("Ben5K XferAllFiles (" + siteName + "): " + ex.ToString());
                Program.Log("Ben5K XferAllFiles (" + siteName + "): " + ex.ToString());
                return false;
            }
        }

        private void XferDataFiles()
        {
            List<BenRecord> myFiles = GetFileList();
            int curRecId = downloadFileNumber;

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
                        FixCFGFiles(myFiles);
                        curRecId = myFiles[0].rId;

                    }
                    else
                    {
                        SendTooManyFilesEmailNotification(numFiles);
                        throw new System.Exception("Site " + siteName + " has too many files, aborting download.");
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Log("XFER Error/Site: " + siteName + " - " + ex.ToString());

                throw new System.Exception("XFER Error/Site: " + siteName + " - " + ex.ToString());

            }
            finally
            {
                lastFileDownloaded = curRecId.ToString();

            }
        }

        private void FixCFGFiles(List<BenRecord> fileList)
        {
            try
            {
                string curFileName;
                string curFileContent;
                foreach (BenRecord curRec in fileList)
                {
                    curFileName = localPath + "\\" + get232FN(curRec.rDateTime, serialNumber) + ".cfg";
                    if (FileSystem.FileExists(curFileName))
                    {
                        curFileContent = FileSystem.ReadAllText(curFileName);
                        FileSystem.WriteAllText(curFileName, siteName + curFileContent, false, System.Text.Encoding.ASCII);
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Log("fixcfgFiles error: " + siteName + " - " + ex.ToString());
                throw new System.Exception("fixcfgFiles error: " + siteName + " - " + ex.ToString());
            }

        }

        private List<BenRecord> GetFileList()
        {
            List<BenRecord> downloadList = new List<BenRecord>();
            string dirFile = localPath + "\\bendir.txt";

            try
            {
                //delete the existing dir file if one exists.
                //if (FileSystem.FileExists(dirFile))
                //    FileSystem.DeleteFile(dirFile);

                //build new dir files.
                BuildBenLinkDirINI();
                
                ExecBenCommand();

                // build list of records to download
                // todo: build an algroithm for rollover of record numbers
                if (FileSystem.FileExists(dirFile))
                {
                    TextFieldParser dirReader = FileSystem.OpenTextFieldParser(dirFile);
                    string[] curRow;
                    while (!dirReader.EndOfData)
                    {
                        curRow = dirReader.ReadFields();
                        if (Convert.ToInt32(curRow[0]) > Convert.ToInt32(lastFileDownloaded))
                        {
                            if (Convert.ToInt32(curRow[2]) < BENMAXFILESIZE)
                            {
                                BenRecord curRecord = new BenRecord(Convert.ToInt32(curRow[0]), Convert.ToDateTime(curRow[1]), Convert.ToInt32(curRow[2]));
                                downloadList.Add(curRecord);
                            }
                            else
                            {
                                SendFileTooLargeEmailNotification("Record id: " + curRow[0]);
                            }
                        }
                        else
                        {
                            SendFileNumberLargerEmailNotification("Record id: " + curRow[0], downloadFileNumber);
                        }
                    }
                    dirReader.Close();
                }
                else
                {
                    throw new Exception("GetFileList Error: " + siteName + " - dir file does not exist.");
                }

            }
            catch (Exception ex)
            {
                Program.Log("GetFileList Error: " + siteName + " - " + ex.ToString());

                throw new Exception("GetFileList Error: " + siteName + " - " + ex.ToString());
            }
            return downloadList;
        }

        private void ExecBenCommand()
        {
            try
            {
                string benLinCmdLine = ConfigurationFile.Current.Settings["systemSettings"]["BenLinkCommandLine"].Value;
                string cmdLine = benLinCmdLine.Replace("xxx", GetShortPath(localPath));
                string[] cmdLineSplit = cmdLine.Split(new char[] { ' ' }, 2);
                var psi = new ProcessStartInfo(cmdLineSplit[0])
                {
                    Arguments = cmdLineSplit[1],
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                bool flag = true;
                while (flag)
                {
                    using (Process p = Process.Start(psi))
                    {
                        p.WaitForExit();

                        if (p.ExitCode == 0)
                            flag = false;
                    }

                }

                FileSystem.DeleteFile(localPath + "\\benlink.req");
                FileSystem.DeleteFile(localPath + "\\benlink.rsp");
            }
            catch (Exception ex)
            {
                Program.Log("ExecBenCommand error: " + siteName + " - " + ex.ToString());

                throw new Exception("ExecBenCommand error: " + siteName + " - " + ex.ToString());
            }
        }

        private void BuildBenLinkDLINI(List<BenRecord> fileList)
        {
            string requestfilename = localPath + "\\benlink.req";

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
                                "DeviceSN=" + serialNumber + System.Environment.NewLine +
                                "NominalFrequency=60" + System.Environment.NewLine +
                                "DataDirectory=" + localPath + '\\' + System.Environment.NewLine +
                                System.Environment.NewLine +
                                "[ConnectionParam]" + System.Environment.NewLine +
                                "AccessType=0" + System.Environment.NewLine +
                                "UserName=0" + System.Environment.NewLine +
                                "CommAddress=1" + System.Environment.NewLine +
                                "IPAddress=" + ipAddress + System.Environment.NewLine +
                                "HangupTimeout=0";

            int i = 1;
            int curYear = DateTime.Now.Year;

            foreach (BenRecord currec in fileList)
            {
                if (currec.rDateTime.Year > curYear)
                {
                    SendFileFromFutureNotification("Record ID: " + currec.rId);
                    throw new System.Exception("FileID " + currec.rId + " at site " + siteName + " from the future. Fix DFR clock.");
                }

                myINIFile += System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + "[Request" + i++ + "]" + System.Environment.NewLine +
                            "RequestType=2" + System.Environment.NewLine +
                            "RecordNum=" + currec.rId + System.Environment.NewLine +
                            "SubBenNum=0" + System.Environment.NewLine +
                            "Origin=1" + System.Environment.NewLine +
                            "OptionFlags=1" + System.Environment.NewLine +
                            "DataPath=" + localPath + System.Environment.NewLine +
                            "FileName=" + get232FN(currec.rDateTime, serialNumber);
                //"FileName=" +currec.rDateTime.ToString("yyMMdd,HHmmssfff") +"," +tzoffset +"," +Replace(sitename, " ", "_") +"," +siteuser +",TVA"
                //working "FileName=" +siteuser +"-" +currec.rId +"-" +currec.rDateTime.ToString("yyMMdd-HHmmssfff")
                //using the property in the site base class for bens.  
                //if it causes problems with the openFLE we may need to 2 folders one for the aux files and one for the
                //comtrade data itself.  may want to consider making this a property in the app.config file.
                //"DataPath=" +Replace(localPath, "E:\ben", "E:\bencomtrade") +System.Environment.NewLine +_
            }


            try
            {
                FileSystem.WriteAllText(requestfilename, myINIFile, false, System.Text.Encoding.ASCII);
            }
            catch (Exception ex)
            {
                Program.Log("BuildBenLinkDLINI error: " + siteName + " - " + ex.ToString());

                throw new System.Exception("BuildBenLinkDLINI error: " + siteName + " - " + ex.ToString());
            }
        }

        private void BuildBenLinkDirINI()
        {
            string requestFileName = localPath + "\\benlink.req";

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
                                "DeviceSN=" + serialNumber + System.Environment.NewLine +
                                "NominalFrequency=60" + System.Environment.NewLine +
                                "DataDirectory=" + localPath + '\\' + System.Environment.NewLine +
                                System.Environment.NewLine +
                                "[ConnectionParam]" + System.Environment.NewLine +
                                "AccessType=0" + System.Environment.NewLine +
                                "UserName=0" + System.Environment.NewLine +
                                "CommAddress=1" + System.Environment.NewLine +
                                "IPAddress=" + ipAddress + System.Environment.NewLine +
                                "HangupTimeout=0" + System.Environment.NewLine +
                                System.Environment.NewLine +
                                "[Request1]" + System.Environment.NewLine +
                                "RequestType=1" + System.Environment.NewLine +
                                "Origin=1" + System.Environment.NewLine +
                                "SubBens=1" + System.Environment.NewLine +
                                "DataPath=" + localPath;

            try
            {
                FileSystem.WriteAllText(requestFileName, myINIFile, false, System.Text.Encoding.ASCII);
            }
            catch (Exception ex)
            {
                Program.Log("BuildBenLinkDirINI error: " + siteName + '-' + ex.ToString());

                throw new SystemException("BuildBenLinkDirINI error: " + siteName + '-' + ex.ToString());
            }

        }

        #endregion

        #region [File System]
        private string get232FN(DateTime myDate, string myDevID, string timeStampType = "t")
        {
            DateTime rDateUTC = myDate.ToUniversalTime();
            long tzoffset = Math.Abs(DateAndTime.DateDiff(DateInterval.Hour, myDate, rDateUTC)) * -1;
            return myDate.ToString("yyMMdd,HHmmssfff") + "," + tzoffset + timeStampType + "," + siteName.Replace(" ", "_") + "," + serialNumber + ",TVA";
        }

        private int GetFileNumber(string fn)
        {
            string tmpFN = Program.Left(Program.Right(fn, 8), 4);

            int rslt;
            try
            {
                rslt = Convert.ToInt32(tmpFN);
            }
            catch (Exception ex)
            {
                rslt = DownloadFileNumber();
                Program.Log("Get File Number (" + siteName + ")- " + ex.ToString());

                throw new Exception("Get File Number (" + siteName + ")- " + ex.ToString());

            }

            return rslt;
        }

        private int DownloadFileNumber()
        {
            downloadFileNumber = GetFileNumber(lastFileDownloaded);
            return downloadFileNumber;
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


        private void SendEmailDefault(string msgBody)
        {
#if DEBUG
            string msgTo = "tllaughner@tva.gov";
#else
                string msgTo = "powerquality@tva.gov";

#endif
            string msgFrom = "powerquality@tva.gov";
            string msgSubject = "Site " + siteName + " problem";

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
            // todo: addNoteToPQMS(msgSubject & " - " & msgBody)
        }

        //todo
        //        Private Sub addNoteToPQMS(ByVal noteText As String)
        //    Dim pqmscs As String = My.Settings.PQMSConnectString
        //    Dim myConn As New SqlConnection(pqmscs)

        //    Try
        //        Dim sql As String = "select id from site where download_link='" & siteid & "'"
        //        Dim myCMD As SqlCommand = New SqlCommand(sql, myConn)
        //        myConn.Open()
        //        Dim pqmssiteid As Integer = myCMD.ExecuteScalar()
        //        sql = "select count(note_id) from notes where sites_id=" & pqmssiteid & " and entered_date >'" & Date.Now.Date & "'"
        //        myCMD.CommandText = sql
        //        'check to see if there are already notes for today in the database. if so, don't add a new one.
        //        Dim numNotes As Integer = myCMD.ExecuteScalar()

        //        If numNotes< 1 Then
        //            Dim addNoteCMD As SqlCommand = New SqlCommand("insert into notes (sites_id,note,entered_date,entered_by,email_sent_flg) values (@SITEID,@NOTETXT,@UPDATETIME,@DFRID,@EMLFLG)", myConn)

        //            With addNoteCMD
        //                .Parameters.Add("@SITEID", SqlDbType.Int).Value = pqmssiteid
        //                .Parameters.Add("@NOTETXT", SqlDbType.NText).Value = noteText
        //                .Parameters.Add("@UPDATETIME", SqlDbType.DateTime).Value = Date.Now
        //                .Parameters.Add("@DFRID", SqlDbType.NVarChar).Value = "DFRDL"
        //                .Parameters.Add("@EMLFLG", SqlDbType.NVarChar).Value = "*"
        //                Dim addNote As Integer = .ExecuteNonQuery
        //            End With

        //        End If

        //    Catch ex As Exception
        //        Throw New System.Exception("PQMS Note Failed for site id " & siteid & " - " & ex.ToString)
        //    Finally
        //        If myConn.State = ConnectionState.Open Then
        //            myConn.Close()
        //        End If
        //    End Try
        //End Sub


        #endregion

    }

}
