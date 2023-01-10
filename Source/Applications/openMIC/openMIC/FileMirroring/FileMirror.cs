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
using GSF.Threading;
using openMIC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MirrorHandlers = System.Collections.Generic.List<openMIC.FileMirroring.MirrorHandler>;

namespace openMIC.FileMirroring
{
    /// <summary>
    /// Defines an enumeration of file operations that <see cref="FileMirror"/> can handle.
    /// </summary>
    public enum FileOperation
    {
        /// <summary>
        /// Copies file to remote location. Folders will be created as needed.
        /// </summary>
        Copy,
        /// <summary>
        /// Deletes file from remote location. Folders will be removed when empty.
        /// </summary>
        Delete
    }

    /// <summary>
    /// Represents a file mirroring operation.
    /// </summary>
    public class FileMirror
    {
        #region [ Members ]

        // Fields
        private readonly AsyncQueue<List<Action>> m_operationQueue;
        private readonly Dictionary<string, MirrorHandlers> m_mirrors;
        private readonly DelayedSynchronizedOperation m_loadMirrors;
        private DateTime m_lastMirrorUpdate;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="FileMirror"/>.
        /// </summary>
        public FileMirror()
        {
            m_operationQueue = new AsyncQueue<List<Action>>(SynchronizedOperationType.LongBackground)
            {
                ProcessItemFunction = ProcessFileOperations,
                Enabled = true
            };

            m_mirrors = new Dictionary<string, MirrorHandlers>(StringComparer.OrdinalIgnoreCase);

            // Wait 5 seconds before starting new synchronized load operations
            m_loadMirrors = new DelayedSynchronizedOperation(LoadMirrors, LogException) { Delay = 5000 };

            try
            {
                // Load initial output mirrors without delay
                LoadMirrors();
            }
            catch (Exception ex)
            {
                Program.Host.LogException(ex);
            }
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
        public void Load() => 
            m_loadMirrors.RunOnceAsync();

        private void LoadMirrors()
        {
            using AdoDataConnection connection = new("systemSettings");
            TableOperations<OutputMirror> tableOperations = new(connection);
            OutputMirror[] outputMirrors = tableOperations.QueryRecords().ToArray();

            if (outputMirrors.Length == 0)
                return;

            DateTime lastMirrorUpdate = outputMirrors.Select(outputMirror => outputMirror.UpdatedOn).Max();

            // Cancel load operation if no output mirrors have been updated since last load
            if (lastMirrorUpdate <= m_lastMirrorUpdate)
                return;

            m_lastMirrorUpdate = lastMirrorUpdate;

            Dictionary<string, MirrorHandlers> mirrors = new(StringComparer.OrdinalIgnoreCase);

            foreach (OutputMirror outputMirror in outputMirrors)
            {
                // Each source location can have multiple mirror handlers
                MirrorHandlers handlers = mirrors.GetOrAdd(outputMirror.Source, _ => new MirrorHandlers());
                
                switch (outputMirror.Type)
                {
                    case OutputMirrorConnectionType.FileSystem:
                        handlers.Add(new FileSystemMirror(outputMirror)
                        {
                            LogStatusMessageFunction = LogStatusMessageFunction,
                            LogExceptionFunction = LogExceptionFunction
                        });
                        break;
                    case OutputMirrorConnectionType.FTP:
                        handlers.Add(new FTPMirror(outputMirror)
                        {
                            LogStatusMessageFunction = LogStatusMessageFunction,
                            LogExceptionFunction = LogExceptionFunction
                        });
                        break;
                    case OutputMirrorConnectionType.SFTP:
                        handlers.Add(new SFTPMirror(outputMirror)
                        {
                            LogStatusMessageFunction = LogStatusMessageFunction,
                            LogExceptionFunction = LogExceptionFunction
                        });
                        break;
                    case OutputMirrorConnectionType.UNC:
                        handlers.Add(new UNCMirror(outputMirror)
                        {
                            LogStatusMessageFunction = LogStatusMessageFunction,
                            LogExceptionFunction = LogExceptionFunction
                        });
                        break;
                    default:
                        LogStatusMessage(UpdateType.Warning, $"Encounter unexpected output mirror type: {outputMirror.Type}");
                        break;
                }
            }

            lock (m_mirrors)
            {
                // Dispose existing handlers - this will close any open connections
                foreach (MirrorHandlers handlers in m_mirrors.Values)
                {
                    foreach (IDisposable handler in handlers)
                        handler.Dispose();
                }

                m_mirrors.Clear();

                foreach (KeyValuePair<string, MirrorHandlers> mirror in mirrors)
                    m_mirrors.Add(mirror.Key, mirror.Value);
            }
        }

        /// <summary>
        /// Queues any configured mirror operations for downloaded <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">Download filename to mirror.</param>
        /// <param name="operation">File operation for mirror filename.</param>
        public void Queue(string filePath, FileOperation operation)
        {
            List<Action> operations = new();
            Dictionary<string, MirrorHandlers> mirrors;

            lock (m_mirrors)
                mirrors = m_mirrors;

            IEnumerable<MirrorHandlers> targetHandlers = mirrors.
                Where(mirror => filePath.StartsWith(mirror.Key, StringComparison.OrdinalIgnoreCase)).
                Select(mirror => mirror.Value);

            foreach (MirrorHandlers handlers in targetHandlers)
            {
                operations.AddRange(handlers.Select(handler => new Action(() =>
                {
                    switch (operation)
                    {
                        case FileOperation.Copy:
                            handler.CopyFile(filePath);
                            break;
                        case FileOperation.Delete:
                            handler.DeleteFile(filePath);
                            break;
                        default:
                            return;
                    }
                })));
            }

            if (operations.Any())
                m_operationQueue.Enqueue(operations);
        }

        private void ProcessFileOperations(List<Action> actions)
        { 
            // Actions will be for same source, but different output mirrors - so parallel handling is considered safe
            Parallel.ForEach(actions, action =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    LogException(ex);
                }
            });
        }

        private void LogStatusMessage(UpdateType updateType, string message) => 
            LogStatusMessageFunction?.Invoke($"[{nameof(FileMirror)}] {message}", updateType);

        private void LogException(Exception ex) => 
            LogExceptionFunction?.Invoke(new InvalidOperationException($"[{nameof(FileMirror)}] ERROR: {ex.Message}", ex));

        #endregion
    }
}