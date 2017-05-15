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

using System;
using System.Dynamic;

namespace openMIC.Model
{
    public enum ProgressState
    {
        Queued,
        Processing,
        PartialSuccess,
        Success,
        Fail
    }

    public class ProgressUpdate
    {
        public ProgressState? State
        {
            get;
            set;
        }

        public long? OverallProgress
        {
            get;
            set;
        }

        public long? OverallProgressTotal
        {
            get;
            set;
        }

        public long? Progress
        {
            get;
            set;
        }

        public long? ProgressTotal
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        public string Summary
        {
            get;
            set;
        }

        public object AsExpandoObject()
        {
            dynamic obj = new ExpandoObject();

            if (State != null)
                obj.State = State;

            if (OverallProgress != null)
                obj.OverallProgress = OverallProgress;

            if (OverallProgressTotal != null)
                obj.OverallProgressTotal = OverallProgressTotal;

            if (Progress != null)
                obj.Progress = Progress;

            if (ProgressTotal != null)
                obj.ProgressTotal = ProgressTotal;

            if (Message != null)
                obj.Message = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] {Message}";

            if (ErrorMessage != null)
                obj.ErrorMessage = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] {ErrorMessage}";

            if (Summary != null)
                obj.Summary = Summary;

            return obj;
        }
    }
}