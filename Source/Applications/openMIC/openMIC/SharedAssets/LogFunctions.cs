//******************************************************************************************************
//  LogFunctions.cs - Gbtc
//
//  Copyright © 2021, Grid Protection Alliance.  All Rights Reserved.
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
//  06/21/2021 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;

// ReSharper disable once CheckNamespace
namespace openMIC.SharedAssets
{
    // This file is shared between .NET Core and .NET Framework, make sure changes compile on both platforms

    /// <summary>
    /// Defines shared log operations, functions and constants that are shared between openMIC and external
    /// operations, e.g., BenDownloader, IONDownloader and DranetzDownloader.
    /// </summary>
    public static class LogFunctions
    {
        public const string HostService = nameof(openMIC);

        public const string OperationSeparator = " :: ";

        public const string OperationPrefix = HostService + OperationSeparator;

        public const string OperationSuffix = OperationSeparator + "{0}";

        public const string LogDownloadedFileTemplate = OperationPrefix  + "Log Downloaded File" + OperationSuffix;
    
        public const string LogConnectionSuccessTemplate = OperationPrefix + "Log Connection Success" + OperationSuffix;

        public const string LogConnectionFailureTemplate = OperationPrefix + "Log Connection Failure" + OperationSuffix;

        public static void LogDownloadedFile(string fileName) => Console.WriteLine(LogDownloadedFileTemplate, fileName);

        public static void LogConnectionSuccess(string message) => Console.WriteLine(LogConnectionSuccessTemplate, message);

        public static void LogConnectionFailure(string message) => Console.WriteLine(LogConnectionFailureTemplate, message);
    }
}
