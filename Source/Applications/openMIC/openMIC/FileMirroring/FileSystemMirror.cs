//******************************************************************************************************
//  FileSystemMirror.cs - Gbtc
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

using System.IO;
using System.Linq;
using openMIC.Model;

namespace openMIC.FileMirroring
{
    /// <summary>
    /// Represents the an openMIC file mirror handler for file system locations.
    /// </summary>
    public class FileSystemMirror : MirrorHandler
    {
        /// <summary>
        /// Creates a new <see cref="FileSystemMirror"/>.
        /// </summary>
        /// <param name="config">Output mirror configuration loaded from the database.</param>
        public FileSystemMirror(OutputMirror config) : base(config)
        {
        }

        /// <summary>
        /// Gets connection type for output mirror handler instance.
        /// </summary>
        public override OutputMirrorConnectionType Type => OutputMirrorConnectionType.FileSystem;

        /// <summary>
        /// Gets the remote directory character, e.g., "/" or "\".
        /// </summary>
        public override string RemoteDirChar => @"\";

        /// <summary>
        /// Copies <paramref name="filePath"/> to configured destination.
        /// Folders in path should be created.
        /// </summary>
        /// <param name="filePath">File to copy.</param>
        protected override void CopyFileInternal(string filePath)
        {
            string destination = GetRemoteFilePath(filePath);

            if (!Directory.Exists(Path.GetDirectoryName(destination)))
                Directory.CreateDirectory(Path.GetDirectoryName(destination)!);

            File.Copy(filePath, destination, true);
        }

        /// <summary>
        /// Derived class implementation of function that deletes <paramref name="filePath"/> from configured destination.
        /// Empty folders should be deleted.
        /// </summary>
        /// <param name="filePath">File to delete.</param>
        protected override void DeleteFileInternal(string filePath)
        {
            string destination = GetRemoteFilePath(filePath);
            string destinationDir = Path.GetDirectoryName(destination);

            if (!Directory.Exists(destinationDir))
                return;

            File.Delete(destination);
                
            if (!Directory.EnumerateFiles(destinationDir).Any())
                Directory.Delete(destinationDir);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
        }
    }
}