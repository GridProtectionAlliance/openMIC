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
using System.Net.Http;
using GSF;
using GSF.Configuration;
using GSF.Data;
using GSF.Data.Model;
using GSF.IO;
using GSF.Parsing;
using Ionic.Zip;
using openMIC.Model;

namespace IGridDownloader
{
    public static class Program
    {
        private const string ExportEventListURL = "{0}&action=exportData&format=csv&zipFormat=off&serialNumber={1}&startDate={2:yyyy-MM-dd}&endDate={3:yyyy-MM-dd}";
        private const string ExportDataURL = "{0}&action=exportData&format=pqdif&zipFormat=on&serialNumber={1}&eventId={2}";

        private static string s_baseUrl;
        private static string s_localPath;
        private static string s_serialNumber;

        public static void Main(string[] args)
        {
            try
            {
                if (args.Length < 2)
                    throw new ArgumentOutOfRangeException($"Expected 2 command line parameters, received {args.Length}.");

                int deviceID = int.Parse(args[0]);
                int profileTaskID = int.Parse(args[1]);
                long processedFiles = 0;

                DateTime startTime = DateTime.Today.AddDays(-2.0D);
                DateTime endTime = DateTime.Today.AddDays(1.0D);

                if (args.Length >= 4)
                {
                    startTime = ParseTimeTag(args[2]);
                    endTime = ParseTimeTag(args[3]);
                }

                using (AdoDataConnection connection = OpenDatabaseConnection())
                {
                    ParseConfigurationSettings(connection, deviceID, profileTaskID);

                    Console.WriteLine($"Target download folder = {FilePath.TrimFileName(s_localPath, 40)}");

                    using (HttpClient client = new HttpClient())
                    {
                        const string EventIDKey = "EventId";

                        Func<string, string> getLocalFileName = eventID => $"Event{eventID}.pqd";

                        Console.WriteLine("Downloading list of events from I-Grid web service...");

                        string eventInfo = client.GetStringAsync(string.Format(ExportEventListURL, s_baseUrl, s_serialNumber, startTime, endTime)).Result;

                        string[][] table = eventInfo.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)
                            .Select(line => line.Split('\t'))
                            .ToArray();

                        List<Dictionary<string, string>> eventList = table
                            .Select(line => new Dictionary<string, string>())
                            .ToList();

                        for (int i = 1; i < table.Length; i++)
                        {
                            int length = Math.Min(table[0].Length, table[i].Length);

                            for (int j = 0; j < length; j++)
                                eventList[i].Add(table[0][j], table[i][j]);
                        }

                        eventList.RemoveAll(evt => evt.Count == 0 || File.Exists(Path.Combine(s_localPath, getLocalFileName(evt[EventIDKey]))));

                        if (eventList.Count == 0)
                        {
                            Console.WriteLine("No new data available for download.");
                            return;
                        }

                        string tempDirectoryPath = GetTempDirectoryPath();

                        Console.WriteLine($"Downloading {eventList.Count} files...");

                        for (int i = 0; i < eventList.Count; i++)
                        {
                            string eventID = eventList[i][EventIDKey];
                            string localFileName = getLocalFileName(eventID);
                            string localFilePath = Path.Combine(s_localPath, localFileName);
                            string tempFilePath = Path.Combine(tempDirectoryPath, localFileName);
                            string address = string.Format(ExportDataURL, s_baseUrl, s_serialNumber, eventID);

                            using (MemoryStream webStream = new MemoryStream(client.GetByteArrayAsync(address).Result))
                            using (ZipFile zipFile = ZipFile.Read(webStream))
                            using (Stream zipEntry = zipFile.Entries.First().OpenReader())
                            using (FileStream fileStream = File.Create(tempFilePath))
                            {
                                zipEntry.CopyTo(fileStream);
                            }

                            File.Move(tempFilePath, localFilePath);
                            Console.WriteLine($"openMIC :: Log Downloaded File :: {Path.Combine(s_localPath, localFileName)}");
                            Console.WriteLine($"Downloaded \"{localFileName}\", {++processedFiles} out of {eventList.Count} files complete...");
                        }

                        Directory.Delete(tempDirectoryPath);
                        Console.WriteLine($"Completed downloading {processedFiles} files.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Download Failure: {ex.Message}");
                Environment.Exit(1);
            }
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
            profileTaskTable.RootQueryRestriction[0] = profile.ID;

            ConnectionProfileTask profileTask = profileTaskTable.QueryRecordWhere("ID = {0}", profileTaskID);
            ConnectionProfileTaskSettings profileTaskSettings = profileTask.Settings;

            s_baseUrl = deviceConnectionString["baseURL"];
            s_serialNumber = deviceConnectionString["serialNumber"];

            string subFolder = GetSubFolder(device, profile.Name, profileTaskSettings.DirectoryNamingExpression);
            s_localPath = $"{profileTaskSettings.LocalPath}{Path.DirectorySeparatorChar}{subFolder}";

            if (!Directory.Exists(s_localPath))
                Directory.CreateDirectory(s_localPath);
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

            return $"{directoryNameExpressionParser.Execute(substitutions)}{Path.DirectorySeparatorChar}";
        }

        private static string GetTempDirectoryPath()
        {
            string tempDirectoryPath = null;

            while (true)
            {
                try
                {
                    tempDirectoryPath = Path.Combine(Path.GetTempPath(), "IGridDownloader_" + Path.GetRandomFileName());
                    Directory.CreateDirectory(tempDirectoryPath);
                    break;
                }
                catch (IOException)
                {
                }
            }

            return tempDirectoryPath;
        }

        private static DateTime ParseTimeTag(string timeTag)
        {
            DateTime dateTime;

            if (DateTime.TryParse(timeTag, out dateTime))
                return dateTime;

            string strippedTimeTag = new string(timeTag.Where(c => !char.IsWhiteSpace(c)).ToArray());

            if (!strippedTimeTag.StartsWith("*"))
                throw new FormatException($"\"{timeTag}\" is not recognized as a valid time tag.");

            if (strippedTimeTag.Length == 1)
                return DateTime.Today;

            char unit = strippedTimeTag.Last();
            int offset;

            if (!int.TryParse(strippedTimeTag.Substring(1, strippedTimeTag.Length - 2), out offset))
                throw new FormatException($"\"{timeTag}\" is not recognized as a valid time tag.");

            switch (unit)
            {
                case 'D': case 'd': return DateTime.Today.AddDays(offset);
                case 'W': case 'w': return DateTime.Today.AddDays(offset * 7);
                case 'M': case 'm': return DateTime.Today.AddMonths(offset);
                case 'Y': case 'y': return DateTime.Today.AddYears(offset);
                default: throw new FormatException($"\"{timeTag}\" is not recognized as a valid time tag.");
            }
        }
    }
}