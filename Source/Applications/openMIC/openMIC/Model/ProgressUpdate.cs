//******************************************************************************************************
//  ProgressUpdate.cs - Gbtc
//
//  Copyright © 2017, Grid Protection Alliance.  All Rights Reserved.
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
//  05/25/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

namespace openMIC.Model
{
    public enum ProgressState
    {
        Queued,
        Processing,
        Skipped,
        Succeeded,
        Failed,
        Finished
    }

    public class ProgressUpdate
    {
        public ProgressUpdate()
        {            
        }

        public ProgressUpdate(ProgressState state, bool targetIsOverall, string progressMessage, long progressComplete, long progressTotal)
        {
            State = state;
            TargetIsOverall = targetIsOverall;
            ProgressMessage = progressMessage;
            ProgressComplete = progressComplete;
            ProgressTotal = progressTotal;
        }

        public bool TargetIsOverall
        {
            get;
            set;
        }

        public ProgressState State
        {
            get;
            set;
        }

        public string DeviceName
        {
            get;
            set;
        }

        public long ValuesProcessed
        {
            get;
            set;
        }

        public long FilesDownloaded
        {
            get;
            set;
        }

        public long TotalFilesDownloaded
        {
            get;
            set;
        }

        public long ProgressComplete
        {
            get;
            set;
        }

        public long ProgressTotal
        {
            get;
            set;
        }

        public string ProgressMessage
        {
            get;
            set;
        }
    }
}