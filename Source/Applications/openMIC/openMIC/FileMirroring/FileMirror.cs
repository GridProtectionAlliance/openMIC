//******************************************************************************************************
//  FileMirror.cs - Gbtc
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

using GSF;
using GSF.Collections;
using GSF.Data;
using GSF.Data.Model;
using openMIC.Model;
using System;
using System.Collections.Generic;

namespace openMIC.FileMirroring
{
    /// <summary>
    /// Represents a file mirroring operation.
    /// </summary>
    public class FileMirror
    {
        #region [ Members ]

        // Nested Types

        // Constants

        // Delegates

        // Events

        // Fields
        private readonly Dictionary<string, List<MirrorHandler>> m_mirrors;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="FileMirror"/>.
        /// </summary>
        public FileMirror()
        {
            m_mirrors = new Dictionary<string, List<MirrorHandler>>(StringComparer.OrdinalIgnoreCase);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets delegate to use to log status messages, if any.
        /// </summary>
        public Action<string, UpdateType> LogStatusMessageFunction { get; set; }

        /// <summary>
        /// Gets or sets delegate to use to log exceptions, if any.
        /// </summary>
        public Action<Exception> LogExceptionFunction { get; set; }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Loads all existing mirror configurations.
        /// </summary>
        public void Load()
        {
            using AdoDataConnection connection = new("systemSettings");
            TableOperations<OutputMirror> tableOperations = new(connection);
            IEnumerable<OutputMirror> outputMirrors = tableOperations.QueryRecords();

            Dictionary<string, List<MirrorHandler>> mirrors = new(StringComparer.OrdinalIgnoreCase);

            foreach (OutputMirror outputMirror in outputMirrors)
            {
                // Each source location can have multiple mirror handlers
                List<MirrorHandler> handlers = mirrors.GetOrAdd(outputMirror.Source, _ => new List<MirrorHandler>());
                
                switch (outputMirror.Type)
                {
                    case OutputMirrorConnectionType.FileSystem:
                        handlers.Add(new FileSystemMirror(outputMirror));
                        break;
                    case OutputMirrorConnectionType.FTP:
                        handlers.Add(new FTPMirror(outputMirror));
                        break;
                    case OutputMirrorConnectionType.SFTP:
                        handlers.Add(new SFTPMirror(outputMirror));
                        break;
                    case OutputMirrorConnectionType.UNC:
                        handlers.Add(new UNCMirror(outputMirror));
                        break;
                    default:
                        LogStatusMessage(UpdateType.Warning, $"Encounter unexpected output mirror type: {outputMirror.Type}");
                        break;
                }
            }

            lock (m_mirrors)
            {
                m_mirrors.Clear();

                foreach (KeyValuePair<string, List<MirrorHandler>> mirror in mirrors)
                    m_mirrors.Add(mirror.Key, mirror.Value);
            }
        }

        /// <summary>
        /// Queues any configured mirror operations for downloaded <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">Download filename to mirror.</param>
        public void Queue(string filePath)
        {

        }

        private void LogStatusMessage(UpdateType updateType, string message) => 
            LogStatusMessageFunction?.Invoke($"[{nameof(FileMirror)}] {message}", updateType);

        private void LogException(Exception ex) => 
            LogExceptionFunction?.Invoke(new InvalidOperationException($"[{nameof(FileMirror)}] ERROR: {ex.Message}", ex));

        #endregion

        #region [ Operators ]

        #endregion

        #region [ Static ]

        // Static Fields

        // Static Constructor

        // Static Properties

        // Static Methods

        #endregion
    }
}