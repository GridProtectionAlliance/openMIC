﻿//******************************************************************************************************
//  UNCMirror.cs - Gbtc
//
//  Copyright © 2023, Grid Protection Alliance.  All Rights Reserved.
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
//  12/29/2022 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using GSF.IO;
using openMIC.Model;
using System.IO;

namespace openMIC.FileMirroring
{
    /// <summary>
    /// Represents the an openMIC file mirror handler for UNC paths.
    /// </summary>
    public class UNCMirror : FileSystemMirror
    {
        /// <summary>
        /// Creates a new <see cref="UNCMirror"/>.
        /// </summary>
        /// <param name="config">Output mirror configuration loaded from the database.</param>
        public UNCMirror(OutputMirror config) : base(config)
        {
            OutputMirrorSettings settings = config.Settings;

            if (!string.IsNullOrWhiteSpace(settings.Username))
                FilePath.ConnectToNetworkShare(settings.Host, settings.Username, settings.Password, null);
        }

        /// <summary>
        /// Gets connection type for output mirror handler instance.
        /// </summary>
        public override OutputMirrorConnectionType Type => OutputMirrorConnectionType.UNC;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            OutputMirrorSettings settings = Config.Settings;
            
            if (!string.IsNullOrWhiteSpace(settings.Username))
                FilePath.DisconnectFromNetworkShare(settings.Host);
        }

        /// <summary>
        /// Gets the configured remote path.
        /// </summary>
        /// <returns>Configured remote path.</returns>
        /// <exception cref="InvalidOperationException">Remote path is not defined for output mirror.</exception>
        protected override string GetRemotePath() => 
            Path.Combine(Config.Settings.Host, base.GetRemotePath());
    }
}