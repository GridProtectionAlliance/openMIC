//******************************************************************************************************
//  DataHub_IGrid.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  05/19/2021 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System.Collections.Generic;
using System.Xml.Linq;
using GSF;
using GSF.Data.Model;
using openMIC.Model;

namespace openMIC;

public partial class DataHub
{
    public const string DefaultIGridConnectionProfileName = "I-Grid Connection Profile";
    public const string DefaultIGridConnectionProfileTaskQueueName = "I-Grid Connection Profile Task Queue";

    public int GetDefaultIGridProfileID() => DataContext.Connection.ExecuteScalar<int?>("SELECT ID FROM ConnectionProfile WHERE Name={0}", DefaultIGridConnectionProfileName) ?? 0;

    public IEnumerable<IGridDevice> QueryIGridDevices(string baseURL)
    {
        TableOperations<Device> deviceTable = DataContext.Table<Device>();
        XDocument document = XDocument.Load($"{baseURL}&action=getMonitorList");

        foreach (XElement monitor in document.Descendants("monitor"))
        {
            XElement location = monitor.Element("location");
            XElement identification = monitor.Element("identification");
            XElement model = monitor.Element("model");

            string serialNumber = identification?.Element("serialNumber")?.Value;
            string modelNumber = model?.Element("number")?.Value;
            string monitorName = identification?.Element("monitorName")?.Value;

            if (string.IsNullOrWhiteSpace(serialNumber) || string.IsNullOrWhiteSpace(modelNumber))
                continue;

            Device deviceRecord = deviceTable.QueryRecordWhere("OriginalSource = {0}", serialNumber);

            if (deviceRecord is null)
            {
                deviceRecord = deviceTable.NewRecord();

                string acronym = monitorName ?? modelNumber;

                // Get a clean acronym
                acronym = acronym
                          .ToUpperInvariant()
                          .ReplaceCharacters(' ', s_isInvalidAcronymChar)
                          .RemoveDuplicateWhiteSpace();

                // Truncate at any numbers in the acronym, note this is based on sample data naming convention
                int index = acronym.IndexOfAny(s_digits);

                if (index > 0)
                    acronym = acronym.Substring(0, index);

                acronym = acronym.Trim().ReplaceWhiteSpace('-');

                deviceRecord.Acronym = $"{acronym}${serialNumber}";
            }

            decimal.TryParse(location?.Element("longitude")?.Value, out decimal longitude);
            decimal.TryParse(location?.Element("latitude")?.Value, out decimal latitude);

            yield return new()
            {
                DeviceID = deviceRecord.ID,
                Acronym = deviceRecord.Acronym,
                SerialNumber = serialNumber,
                Name = monitorName,
                Description = model.Element("description")?.Value,
                ModelNumber = modelNumber,
                Longitude = longitude,
                Latitude = latitude,
                Selected = deviceRecord.ID == 0
            };
        }
    }

    public int GetDefaultIGridConnectionProfileID()
    {
        TableOperations<ConnectionProfileTaskQueue> profileTaskQueueTable = DataContext.Table<ConnectionProfileTaskQueue>();
        TableOperations<ConnectionProfile> profileTable = DataContext.Table<ConnectionProfile>();
        ConnectionProfile profile = profileTable.QueryRecordWhere("Name = {0}", DefaultIGridConnectionProfileName);

        if (profile is null)
        {
            ConnectionProfileTaskQueue profileTaskQueue = profileTaskQueueTable.NewRecord();
            profileTaskQueue.Name = DefaultIGridConnectionProfileTaskQueueName;
            profileTaskQueue.MaxThreadCount = 4;
            profileTaskQueue.UseBackgroundThreads = false;
            profileTaskQueue.Description = "Connection Profile Task for I-Grid Devices";
            profileTaskQueueTable.AddNewRecord(profileTaskQueue);
            profileTaskQueue = profileTaskQueueTable.QueryRecordWhere("Name = {0}", DefaultIGridConnectionProfileTaskQueueName);

            profile = profileTable.NewRecord();
            profile.Name = DefaultIGridConnectionProfileName;
            profile.Description = "Connection Profile for I-Grid Devices";
            profile.DefaultTaskQueueID = profileTaskQueue.ID;
            profileTable.AddNewRecord(profile);
            profile.ID = GetDefaultIGridConnectionProfileID();

            TableOperations<ConnectionProfileTask> profileTaskTable = DataContext.Table<ConnectionProfileTask>();
            RecordRestriction filter = ConnectionProfileTask.CreateFilter(profile.ID);
            int taskCount = profileTaskTable.QueryRecordCount(filter);

            if (taskCount == 0)
            {
                ConnectionProfileTask profileTask = profileTaskTable.NewRecord();
                ConnectionProfileTaskSettings profileTaskSettings = profileTask.Settings;

                profileTask.ConnectionProfileID = profile.ID;
                profileTask.Name = "I-Grid Default Downloader Task";

                profileTaskSettings.FileExtensions = "*.*";
                profileTaskSettings.RemotePath = "/";
                profileTaskSettings.LocalPath = Program.Host.Model.Global.DefaultLocalPath;
                profileTaskSettings.ExternalOperation = "IGridDownloader.exe <DeviceID> <TaskID>";
                profileTaskSettings.DirectoryNamingExpression = @"<YYYY><MM>\<DeviceFolderName>";

                profileTaskTable.AddNewRecord(profileTask);
            }
        }

        return profile.ID;
    }
}