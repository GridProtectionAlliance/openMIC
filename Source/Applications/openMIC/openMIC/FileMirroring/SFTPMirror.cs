//******************************************************************************************************
//  SFTPMirror.cs - Gbtc
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
using GSF.Threading;
using openMIC.Model;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace openMIC.FileMirroring
{
    /// <summary>
    /// Represents the an openMIC file mirror handler for SFTP hosts.
    /// </summary>
    public class SFTPMirror : MirrorHandler
    {
        private readonly SftpClient m_client;
        private readonly Timer m_keepAliveTimer;
        private readonly DelayedSynchronizedOperation m_reconnect;

        /// <summary>
        /// Default buffer size for file operations.
        /// </summary>
        public const int DefaultBufferSize = 32768;

        /// <summary>
        /// Default operation timeout for file operations.
        /// </summary>
        public const int DefaultOperationTimeout = -1;

        /// <summary>
        /// Default keep alive interval for SFTP connections.
        /// </summary>
        public const int DefaultKeepAliveInterval = 10000;

        /// <summary>
        /// Default reconnect delay in milliseconds.
        /// </summary>
        public const int DefaultReconnectDelay = 10000;

        /// <summary>
        /// Creates a new <see cref="SFTPMirror"/>.
        /// </summary>
        /// <param name="config">Output mirror configuration loaded from the database.</param>
        public SFTPMirror(OutputMirror config) : base(config)
        {
            OutputMirrorSettings settings = config.Settings;

            if (string.IsNullOrEmpty(settings.KeyFile))
            {
                m_client = new SftpClient(settings.Host, settings.Port ?? 22, settings.Username, settings.Password);
            }
            else
            {
                PrivateKeyFile keyFile = new(settings.KeyFile, string.IsNullOrEmpty(settings.Password) ? null : settings.Password);
                m_client = new SftpClient(settings.Host, settings.Port ?? 22, settings.Username, keyFile);
            }

            Dictionary<string, string> otherSettings = OtherSettings;

            uint bufferSize;
            int operationTimeout, keepAliveInterval, reconnectDelay;

            if (!otherSettings.TryGetValue(nameof(bufferSize), out string value) || !uint.TryParse(value, out bufferSize) || bufferSize == 0)
                bufferSize = DefaultBufferSize;

            if (!otherSettings.TryGetValue(nameof(operationTimeout), out value) || !int.TryParse(value, out operationTimeout) || operationTimeout < -1)
                operationTimeout = DefaultOperationTimeout;

            m_client.BufferSize = bufferSize;
            m_client.OperationTimeout = operationTimeout == -1 ? Timeout.InfiniteTimeSpan : TimeSpan.FromMilliseconds(operationTimeout);

            if (!otherSettings.TryGetValue(nameof(keepAliveInterval), out value) || !int.TryParse(value, out keepAliveInterval) || keepAliveInterval <= 0)
                keepAliveInterval = DefaultKeepAliveInterval;

            m_keepAliveTimer = new Timer(keepAliveInterval);
            m_keepAliveTimer.Elapsed += KeepAliveTimer_Elapsed;
            m_keepAliveTimer.Start();

            if (!otherSettings.TryGetValue(nameof(reconnectDelay), out value) || !int.TryParse(value, out reconnectDelay) || reconnectDelay <= 0)
                reconnectDelay = DefaultReconnectDelay;

            m_reconnect = new DelayedSynchronizedOperation(Reconnect, LogException)
            {
                Delay = reconnectDelay
            };

            Reconnect();
        }

        private void KeepAliveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!Monitor.TryEnter(m_client))
                return;

            try
            {
                m_client.ChangeDirectory(".");
            }
            catch (Exception ex)
            {
                LogException(ex);
                m_reconnect.RunOnceAsync();
            }
            finally
            {
                Monitor.Exit(m_client);
            }
        }

        private void Reconnect()
        {
            lock (m_client)
            {
                if (m_client.IsConnected)
                    return;

                try
                {
                    m_client.Connect();
                    LogStatusMessage(UpdateType.Information, $"Output mirror \"{Config.Name}\" successfully connected to \"{Config.Settings.Host}\" using {Type}.");
                }
                catch (Exception ex)
                {
                    LogException(new InvalidOperationException($"Output mirror \"{Config.Name}\" failed to reconnect to \"{Config.Settings.Host}\" using {Type}: {ex.Message}", ex));
                    m_reconnect.RunOnceAsync();
                }
            }
        }

        /// <summary>
        /// Gets connection type for output mirror handler instance.
        /// </summary>
        public override OutputMirrorConnectionType Type => OutputMirrorConnectionType.SFTP;

        /// <summary>
        /// Gets the remote directory character, e.g., "/" or "\".
        /// </summary>
        public override string RemoteDirChar => "/";

        /// <summary>
        /// Copies <paramref name="filePath"/> to configured destination.
        /// Folders in path should be created.
        /// </summary>
        /// <param name="filePath">File to copy.</param>
        protected override void CopyFileInternal(string filePath)
        {
            string destination = GetRemoteFilePath(filePath);
            string[] directories = destination.Split('/');

            if (directories.Length > 1)
                directories = directories.Take(directories.Length - 1).ToArray();

            try
            {
                lock (m_client)
                {
                    m_client.ChangeDirectory("/");

                    foreach (string directory in directories)
                    {
                        if (string.IsNullOrEmpty(directory))
                            continue;
                        
                        if (!m_client.Exists(directory))
                            m_client.CreateDirectory(directory);

                        m_client.ChangeDirectory(directory);
                    }
                    
                    using SftpFileStream sftpStream = m_client.Create(destination);
                    using FileStream fileStream = File.OpenRead(filePath);
                    
                    fileStream.CopyTo(sftpStream);
                }
            }
            catch
            {
                m_reconnect.RunOnceAsync();
                throw;
            }
        }

        /// <summary>
        /// Derived class implementation of function that deletes <paramref name="filePath"/> from configured destination.
        /// Empty folders should be deleted.
        /// </summary>
        /// <param name="filePath">File to delete.</param>
        protected override void DeleteFileInternal(string filePath)
        {
            string destination = GetRemoteFilePath(filePath);

            try
            {
                lock (m_client)
                {
                    m_client.DeleteFile(destination);

                    string destinationPath = Path.GetDirectoryName(destination);

                    if (m_client.ListDirectory(destinationPath).Any())
                        return;

                    m_client.DeleteDirectory(destinationPath);
                }
            }
            catch
            {
                m_reconnect.RunOnceAsync();
                throw;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            m_client?.Dispose();
            m_keepAliveTimer?.Dispose();
        }

        /// <summary>
        /// Gets remote path name. Implementations should include relevant host information, if applicable.
        /// </summary>
        /// <returns>Remote path name.</returns>
        protected override string GetRemotePathName() => 
            $"{Config.Settings.Host}/{base.GetRemotePathName()}";
    }
}