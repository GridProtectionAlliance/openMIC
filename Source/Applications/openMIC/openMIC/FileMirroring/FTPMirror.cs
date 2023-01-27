//******************************************************************************************************
//  FTPMirror.cs - Gbtc
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using GSF;
using GSF.Net.Ftp;
using GSF.Threading;
using openMIC.Model;
using Timer = System.Timers.Timer;

namespace openMIC.FileMirroring
{
    /// <summary>
    /// Represents the an openMIC file mirror handler for FTP hosts.
    /// </summary>
    public class FTPMirror : MirrorHandler
    {
        private readonly FtpClient m_client;
        private readonly Timer m_noOpTimer;
        private readonly DelayedSynchronizedOperation m_reconnect;

        /// <summary>
        /// Default connection timeout in milliseconds.
        /// </summary>
        public const int DefaultConnectionTimeout = 30000;

        /// <summary>
        /// Default mode for passive FTP connections.
        /// </summary>
        public const bool DefaultPassive = true;

        /// <summary>
        /// Default host DNS name or IP address to send in FTP PORT command
        /// </summary>
        public const string DefaultActiveAddress = "";

        /// <summary>
        /// Default minimum port in active FTP port range.
        /// </summary>
        public const int DefaultMinActivePort = 0;

        /// <summary>
        /// Default maximum port in active FTP port range.
        /// </summary>
        public const int DefaultMaxActivePort = 0;

        /// <summary>
        /// Default NOOP interval in milliseconds.
        /// </summary>
        public const int DefaultNoOpInterval = 10000;

        /// <summary>
        /// Default reconnect delay in milliseconds.
        /// </summary>
        public const int DefaultReconnectDelay = 10000;

        /// <summary>
        /// Creates a new <see cref="FTPMirror"/>.
        /// </summary>
        /// <param name="config">Output mirror configuration loaded from the database.</param>
        public FTPMirror(OutputMirror config) : base(config)
        {
            OutputMirrorSettings settings = config.Settings;
            
            m_client = new FtpClient
            {
                Server = settings.Host,
                Port = settings.Port ?? 21
            };

            Dictionary<string, string> otherSettings = OtherSettings;

            int connectionTimeout, minActivePort, maxActivePort, noOpInterval, reconnectDelay;
            bool passive = DefaultPassive;
            string activeAddress = DefaultActiveAddress;

            if (!otherSettings.TryGetValue(nameof(connectionTimeout), out string value) || !int.TryParse(value, out connectionTimeout) || connectionTimeout <= 0)
                connectionTimeout = DefaultConnectionTimeout;

            if (otherSettings.TryGetValue(nameof(passive), out value))
                passive = value.ParseBoolean();

            if (otherSettings.TryGetValue(nameof(activeAddress), out value) && !string.IsNullOrEmpty(value))
                activeAddress = value;

            if (!otherSettings.TryGetValue(nameof(minActivePort), out value) || !int.TryParse(value, out minActivePort) || minActivePort < 0)
                minActivePort = DefaultMinActivePort;

            if (!otherSettings.TryGetValue(nameof(maxActivePort), out value) || !int.TryParse(value, out maxActivePort) || maxActivePort < 0)
                maxActivePort = DefaultMaxActivePort;

            m_client.Timeout = connectionTimeout;
            m_client.Passive = passive;
            m_client.ActiveAddress = activeAddress;
            m_client.MinActivePort = minActivePort;
            m_client.MaxActivePort = maxActivePort;

            if (!otherSettings.TryGetValue(nameof(noOpInterval), out value) || !int.TryParse(value, out noOpInterval) || noOpInterval <= 0)
                noOpInterval = DefaultNoOpInterval;

            m_noOpTimer = new Timer(noOpInterval);
            m_noOpTimer.Elapsed += NoOpTimer_Elapsed;
            m_noOpTimer.Start();

            if (!otherSettings.TryGetValue(nameof(reconnectDelay), out value) || !int.TryParse(value, out reconnectDelay) || reconnectDelay <= 0)
                reconnectDelay = DefaultReconnectDelay;

            m_reconnect = new DelayedSynchronizedOperation(Reconnect, LogException)
            {
                Delay = reconnectDelay
            };

            Reconnect();
        }

        private void NoOpTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!Monitor.TryEnter(m_client))
                return;

            try
            {
                m_client.ControlChannel.Command("NOOP");
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
                    m_client.Connect(Config.Settings.Username, Config.Settings.Password);
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
        public override OutputMirrorConnectionType Type => OutputMirrorConnectionType.FTP;

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
                    FtpDirectory current = m_client.RootDirectory;
                    m_client.SetCurrentDirectory(m_client.RootDirectory.FullPath);

                    foreach (string directory in directories)
                    {
                        if (string.IsNullOrEmpty(directory))
                            continue;

                        FtpDirectory next = current.FindSubDirectory(directory);

                        if (next is null)
                        {
                            m_client.ControlChannel.Command($"MKD {directory}");
                            current.Refresh();
                            next = current.FindSubDirectory(directory);
                        }

                        current = next ?? throw new IOException($"Failed to create directory \"{destination}\" on FTP server \"{m_client.Server}\".");
                    }

                    current.PutFile(filePath);
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
            string[] directories = destination.Split('/');

            try
            {
                lock (m_client)
                {
                    FtpDirectory current = m_client.RootDirectory;
                    m_client.SetCurrentDirectory(m_client.RootDirectory.FullPath);

                    foreach (string directory in directories)
                    {
                        if (string.IsNullOrEmpty(directory))
                            continue;

                        FtpDirectory next = current.FindSubDirectory(directory);

                        if (next is null)
                            return;

                        current = next;
                    }

                    current.RemoveFile(Path.GetFileName(filePath));
                    current.Refresh();

                    if (current.Files.Count == 0)
                        current.Parent.RemoveSubDir(current.Name);
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
            m_noOpTimer?.Dispose();
        }

        /// <summary>
        /// Gets remote path name. Implementations should include relevant host information, if applicable.
        /// </summary>
        /// <returns>Remote path name.</returns>
        protected override string GetRemotePathName() =>
            $"{Config.Settings.Host}/{base.GetRemotePathName()}";
    }
}