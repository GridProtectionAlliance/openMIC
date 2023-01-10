//******************************************************************************************************
//  MirrorHandler.cs - Gbtc
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

using openMIC.Model;
using System;
using System.Collections.Generic;
using System.IO;
using GSF;

namespace openMIC.FileMirroring
{
    /// <summary>
    /// Represents the basic functionality required by an openMIC file mirror handler.
    /// </summary>
    public abstract class MirrorHandler : IDisposable
    {
        private string m_remotePath;

        /// <summary>
        /// Output mirror configuration loaded from the database.
        /// </summary>
        protected readonly OutputMirror Config;

        /// <summary>
        /// Creates a new <see cref="MirrorHandler"/>.
        /// </summary>
        /// <param name="config">Output mirror configuration loaded from the database.</param>
        protected MirrorHandler(OutputMirror config)
        {
            if (string.IsNullOrWhiteSpace(config.Source))
                config.Source = "\\";

            config.Source = config.Source.Trim();
            
            if (!config.Source.EndsWith("\\"))
                config.Source += "\\";

            Config = config;
        }

        /// <summary>
        /// Gets connection type for output mirror handler instance.
        /// </summary>
        public abstract OutputMirrorConnectionType Type { get; }

        /// <summary>
        /// Gets the remote directory character, e.g., "/" or "\".
        /// </summary>
        public abstract string RemoteDirChar { get; }
        
        /// <summary>
        /// Gets or sets delegate to use to log status messages, if any.
        /// </summary>
        public Action<string, UpdateType> LogStatusMessageFunction { get; set; }

        /// <summary>
        /// Gets or sets delegate to use to log exceptions, if any.
        /// </summary>
        public Action<Exception> LogExceptionFunction { get; set; }
        
        /// <summary>
        /// Gets any defined custom settings.
        /// </summary>
        protected Dictionary<string, string> OtherSettings =>
            (Config.Settings.OtherSettings ?? "").ParseKeyValuePairs();

        /// <summary>
        /// Copies <paramref name="filePath"/> to configured destination.
        /// Folders in path should be created.
        /// </summary>
        /// <param name="filePath">File to copy.</param>
        public void CopyFile(string filePath)
        {
            if (!Config.Settings.SyncCopy)
                return;

            try
            {
                CopyFileInternal(filePath);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        /// <summary>
        /// Derived class implementation of function that copies <paramref name="filePath"/> to configured destination.
        /// Folders in path should be created.
        /// </summary>
        /// <param name="filePath">File to copy.</param>
        protected abstract void CopyFileInternal(string filePath);

        /// <summary>
        /// Deletes <paramref name="filePath"/> from configured destination.
        /// Empty folders should be deleted.
        /// </summary>
        /// <param name="filePath">File to delete.</param>
        public void DeleteFile(string filePath)
        {
            if (!Config.Settings.SyncDelete)
                return;

            try
            {
                DeleteFileInternal(filePath);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        /// <summary>
        /// Derived class implementation of function that deletes <paramref name="filePath"/> from configured destination.
        /// Empty folders should be deleted.
        /// </summary>
        /// <param name="filePath">File to delete.</param>
        protected abstract void DeleteFileInternal(string filePath);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Gets the remote file path for the source <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">Remote system file path.</param>
        /// <returns>base file name for the source <paramref name="filePath"/>.</returns>
        /// <exception cref="InvalidOperationException">File path does not belong to output mirror.</exception>
        /// <exception cref="InvalidOperationException">Remote path is not defined for output mirror.</exception>
        protected string GetRemoteFilePath(string filePath)
        {
            string baseFilePath = GetBaseFilePath(filePath);

            if (RemoteDirChar != "\\")
                baseFilePath = baseFilePath.Replace("\\", RemoteDirChar);

            return Path.Combine(GetRemotePath(), baseFilePath);
        }

        /// <summary>
        /// Gets the base file path for the source <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">Source system file path.</param>
        /// <returns>base file name for the source <paramref name="filePath"/>.</returns>
        /// <exception cref="InvalidOperationException">File path does not belong to output mirror.</exception>
        protected string GetBaseFilePath(string filePath)
        {
            if (!filePath.StartsWith(Config.Source, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"File path \"{filePath}\" does not belong to output mirror \"{Config.Name}\"");

            return filePath.Substring(Config.Source.Length);
        }

        /// <summary>
        /// Gets the configured remote path.
        /// </summary>
        /// <returns>Configured remote path.</returns>
        /// <exception cref="InvalidOperationException">Remote path is not defined for output mirror.</exception>
        protected string GetRemotePath()
        {
            if (!string.IsNullOrEmpty(m_remotePath))
                return m_remotePath;

            string remotePath = Config.Settings.RemotePath;

            if (string.IsNullOrEmpty(remotePath))
                throw new InvalidOperationException($"Remote path is not defined for output mirror \"{Config.Name}\"");

            if (!remotePath.EndsWith(RemoteDirChar))
                remotePath += RemoteDirChar;

            return m_remotePath = remotePath;
        }
        
        protected void LogStatusMessage(UpdateType updateType, string message) =>
            LogStatusMessageFunction?.Invoke($"[{nameof(FileMirror)}] {message}", updateType);

        protected void LogException(Exception ex) =>
            LogExceptionFunction?.Invoke(new InvalidOperationException($"[{nameof(FileMirror)}] ERROR: {ex.Message}", ex));
    }
}