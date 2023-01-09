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

namespace openMIC.FileMirroring
{
    /// <summary>
    /// Represents the basic functionality required by an openMIC file mirror handler.
    /// </summary>
    public abstract class MirrorHandler : IDisposable
    {
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
            Config = config;
        }

        /// <summary>
        /// Gets connection type for output mirror handler instance.
        /// </summary>
        public abstract OutputMirrorConnectionType Type { get; }

        /// <summary>
        /// Copies <paramref name="file"/> to configured destination.
        /// Folders in path should be created.
        /// </summary>
        /// <param name="file">File to copy.</param>
        public void CopyFile(string file)
        {
            if (!Config.Settings.SyncCopy)
                return;

            CopyFileInternal(file);
        }

        /// <summary>
        /// Derived class implementation of function that copies <paramref name="file"/> to configured destination.
        /// Folders in path should be created.
        /// </summary>
        /// <param name="file">File to copy.</param>
        protected abstract void CopyFileInternal(string file);

        /// <summary>
        /// Deletes <paramref name="file"/> from configured destination.
        /// Empty folders should be deleted.
        /// </summary>
        /// <param name="file">File to delete.</param>
        public void DeleteFile(string file)
        {
            if (!Config.Settings.SyncDelete)
                return;

            DeleteFileInternal(file);
        }

        /// <summary>
        /// Derived class implementation of function that deletes <paramref name="file"/> from configured destination.
        /// Empty folders should be deleted.
        /// </summary>
        /// <param name="file">File to delete.</param>
        protected abstract void DeleteFileInternal(string file);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public abstract void Dispose();
    }
}