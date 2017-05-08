//******************************************************************************************************
//  Program.cs - Gbtc
//
//  Copyright © 2017, Grid Protection Alliance.  All Rights Reserved.
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
//  04/12/2017 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using GSF;
using GSF.Configuration;
using GSF.Data;
using GSF.Data.Model;
using GSF.IO;
using GSF.Parsing;
using openMIC.Model;
using Ionic.Zip;

namespace IGridDownloader
{
    public static class Program
    {
        private const string ExportDataURL = "{0}&action=exportData&daysBack=1&format=pqdif&zipFormat=on&serialNumber={1}";

        private static string s_baseUrl;
        private static string s_localPath;
        private static string s_serialNumber;
        private static string[] s_fileSpecs;
        private static bool s_skipDownloadIfUnchanged;
        private static bool s_overwriteExistingLocalFiles;
        private static bool s_archiveExistingFilesBeforeDownload;
        private static bool s_synchronizeTimestamps;

        private static int Main(string[] args)
        {
            try
            {
                if (args.Length < 2)
                    throw new ArgumentOutOfRangeException($"Expected 2 command line parameters - received {args.Length}.");

                int deviceID = int.Parse(args[0]);
                int profileTaskID = int.Parse(args[1]);
                long processedFiles = 0;

                using (AdoDataConnection connection = OpenDatabaseConnection())
                {
                    ParseConfigurationSettings(connection, deviceID, profileTaskID);

                    string tempZipFile = FilePath.GetUniqueFilePath(Path.Combine(s_localPath, "NewFiles.zip"));

                    try
                    {
                        Console.WriteLine("Downloading latest I-Grid data to zip file...");

                        // Download latest files as a single zip file
                        using (WebClient client = new WebClient())
                            client.DownloadFile(string.Format(ExportDataURL, s_baseUrl, s_serialNumber), tempZipFile);

                        // Process the zip file
                        using (ZipFile zipFile = ZipFile.Read(tempZipFile))
                        {
                            Console.WriteLine($"Processing {zipFile.Count} files from downloaded zip file...");

                            foreach (ZipEntry zipEntry in zipFile)
                            {
                                // Verify that zip file should be processed
                                if (ProcessEntry(zipEntry))
                                    zipEntry.Extract(s_localPath, ExtractExistingFileAction.OverwriteSilently);

                                Console.WriteLine($"Processed \"{zipEntry.FileName}\", {++processedFiles} / {zipFile.Count} complete...");
                            }

                            Console.WriteLine($"Completed processing {processedFiles} files from downloaded zip file.");
                        }
                    }
                    finally
                    {
                        if (File.Exists(tempZipFile))
                            File.Delete(tempZipFile);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Download Failure: {ex.Message}");
                return 1;
            }

            return 0;
        }

        private static AdoDataConnection OpenDatabaseConnection()
        {
            // Access openMIC database configuration settings
            string configFileName = FilePath.GetAbsolutePath("openMIC.exe.config");

            if (!File.Exists(configFileName))
                throw new FileNotFoundException($"Failed to open configuration file \"{configFileName}\" - file does not exist.");

            ConfigurationFile configFile = ConfigurationFile.Open(configFileName);
            CategorizedSettingsSection categorizedSettings = configFile.Settings;
            CategorizedSettingsElementCollection systemSettings = categorizedSettings["systemSettings"];

            return new AdoDataConnection(systemSettings["ConnectionString"]?.Value, systemSettings["DataProviderString"]?.Value);
        }

        private static void ParseConfigurationSettings(AdoDataConnection connection, int deviceID, int profileTaskID)
        {
            TableOperations<Device> deviceTable = new TableOperations<Device>(connection);
            Device device = deviceTable.QueryRecordWhere("ID = {0}", deviceID);
            Dictionary<string, string> deviceConnectionString = device.ConnectionString.ParseKeyValuePairs();

            TableOperations<ConnectionProfile> profileTable = new TableOperations<ConnectionProfile>(connection);
            ConnectionProfile profile = profileTable.QueryRecordWhere("ID = {0}", deviceConnectionString["connectionProfileID"]);

            TableOperations<ConnectionProfileTask> profileTaskTable = new TableOperations<ConnectionProfileTask>(connection);
            ConnectionProfileTask profileTask = profileTaskTable.QueryRecordWhere("ID = {0}", profileTaskID);
            Dictionary<string, string> profileTaskSettings = profileTask.Settings.ParseKeyValuePairs();

            s_baseUrl = deviceConnectionString["baseURL"];
            s_serialNumber = deviceConnectionString["serialNumber"];

            string subFolder = GetSubFolder(device, profile.Name, profileTaskSettings["directoryNamingExpression"]);
            s_localPath = $"{profileTaskSettings["localPath"]}{Path.DirectorySeparatorChar}{subFolder}{Path.DirectorySeparatorChar}".RemoveDuplicates(Path.DirectorySeparatorChar.ToString());

            if (!Directory.Exists(s_localPath))
                Directory.CreateDirectory(s_localPath);

            s_fileSpecs = profileTaskSettings["fileExtensions"].Split(',').Select(pattern => pattern.Trim()).ToArray();
            s_skipDownloadIfUnchanged = profileTaskSettings["skipDownloadIfUnchanged"].ParseBoolean();
            s_overwriteExistingLocalFiles = profileTaskSettings["overwriteExistingLocalFiles"].ParseBoolean();
            s_archiveExistingFilesBeforeDownload = profileTaskSettings["archiveExistingFilesBeforeDownload"].ParseBoolean();
            s_synchronizeTimestamps = profileTaskSettings["synchronizeTimestamps"].ParseBoolean();
        }

        private static bool ProcessEntry(ZipEntry zipEntry)
        {
            // Make sure zip file name matches configured file pattern specifications
            if (!FilePath.IsFilePatternMatch(s_fileSpecs, zipEntry.FileName, true))
            {
                Console.WriteLine($"Skipping file unzip for remote file \"{zipEntry.FileName}\": file name does not match allowed patterns \"{string.Join(", ", s_fileSpecs)}\".");
                return false;
            }

            string localFileName = Path.Combine(s_localPath, zipEntry.FileName);

            // Check for setting that skips download if file has not changed
            if (File.Exists(localFileName) && s_skipDownloadIfUnchanged)
            {
                try
                {
                    FileInfo info = new FileInfo(localFileName);

                    // Compare file sizes and timestamps
                    bool localEqualsRemote =
                        info.Length == zipEntry.UncompressedSize &&
                        (!s_synchronizeTimestamps || info.LastWriteTime == zipEntry.ModifiedTime);

                    if (localEqualsRemote)
                    {
                        Console.WriteLine($"Skipping file unzip for remote file \"{zipEntry.FileName}\": Local file already exists and matches zipped file.");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Unable to determine whether local file size and time matches zip file size and time due to exception: {ex.Message}");
                }
            }

            // Check for setting that archives existing files before downloading (names could be same for different data)
            if (File.Exists(localFileName) && s_archiveExistingFilesBeforeDownload)
            {
                try
                {
                    string directoryName = Path.Combine(FilePath.GetDirectoryName(localFileName), "Archive\\");
                    string archiveFileName = Path.Combine(directoryName, zipEntry.FileName);

                    Directory.CreateDirectory(directoryName);

                    if (File.Exists(archiveFileName))
                        archiveFileName = FilePath.GetUniqueFilePathWithBinarySearch(archiveFileName);

                    Console.WriteLine($"Archiving existing file \"{localFileName}\" to \"{archiveFileName}\"...");
                    File.Move(localFileName, archiveFileName);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Failed to archive existing local file \"{localFileName}\" due to exception: {ex.Message}");
                }
            }

            // Check for setting that skips overwriting of existing local files
            if (File.Exists(localFileName) && !s_overwriteExistingLocalFiles)
            {
                Console.WriteLine($"Skipping file unzip for remote file \"{zipEntry.FileName}\": Local file already exists and settings do not allow overwrite.");
                return false;
            }

            return true;
        }

        private static string GetSubFolder(Device device, string profileName, string directoryNamingExpression)
        {
            TemplatedExpressionParser directoryNameExpressionParser = new TemplatedExpressionParser('<', '>', '[', ']');
            Dictionary<string, string> substitutions = new Dictionary<string, string>
            {
                { "<YYYY>", $"{DateTime.Now.Year}" },
                { "<YY>", $"{DateTime.Now.Year.ToString().Substring(2)}" },
                { "<MM>", $"{DateTime.Now.Month.ToString().PadLeft(2, '0')}" },
                { "<DD>", $"{DateTime.Now.Day.ToString().PadLeft(2, '0')}" },
                { "<DeviceName>", device.Name ?? "undefined"},
                { "<DeviceAcronym>", device.Acronym },
                { "<DeviceFolderName>", string.IsNullOrWhiteSpace(device.OriginalSource) ? device.Acronym : device.OriginalSource},
                { "<ProfileName>", profileName }
            };

            directoryNameExpressionParser.TemplatedExpression = directoryNamingExpression.Replace("\\", "\\\\");

            return FilePath.GetDirectoryName(directoryNameExpressionParser.Execute(substitutions));
        }
    }
}