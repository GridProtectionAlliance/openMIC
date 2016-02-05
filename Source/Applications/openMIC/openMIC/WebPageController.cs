//******************************************************************************************************
//  WebPageController.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  01/12/2016 - Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using GSF.IO;
using GSF.IO.Checksums;

namespace openMIC
{
    /// <summary>
    /// Defines a mini-web server with Razor support using the self-hosted API controller.
    /// </summary>
    public class WebPageController : ApiController
    {
        #region [ Methods ]

        // Default page request handler
        [Route, HttpGet]
        public Task<HttpResponseMessage> GetPage()
        {
            return GetPage(Program.Host.DefaultWebPage);
        }

        // Common request handler
        [Route("{pageName}"), HttpGet]
        public Task<HttpResponseMessage> GetPage(string pageName)
        {
            return RenderResponse(pageName);
        }

        // Common post handler
        [Route("{pageName}"), HttpPost]
        public Task<HttpResponseMessage> PostPage(string pageName, dynamic postData)
        {
            return RenderResponse(pageName, postData);
        }

        private async Task<HttpResponseMessage> RenderResponse(string pageName, dynamic postData = null)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            string content, fileExtension = FilePath.GetExtension(pageName).ToLowerInvariant();

            switch (fileExtension)
            {
                case ".cshtml":
                    content = await new RazorView<CSharp>(pageName, Program.Host.Model).ExecuteAsync(Request, postData);
                    response.Content = new StringContent(content, Encoding.UTF8, "text/html");
                    break;
                case ".vbhtml":
                    content = await new RazorView<VisualBasic>(pageName, Program.Host.Model).ExecuteAsync(Request, postData);
                    response.Content = new StringContent(content, Encoding.UTF8, "text/html");
                    break;
                default:
                    string fileName = FilePath.GetAbsolutePath($"{FilePath.AddPathSuffix(Program.Host.WebRootFolder)}{pageName.Replace('/', Path.DirectorySeparatorChar)}");

                    if (File.Exists(fileName))
                    {
                        FileStream fileData = null;
                        uint responseHash = 0;

                        if (Program.Host.ClientCacheEnabled && !s_etagCache.TryGetValue(fileName, out responseHash))
                        {
                            // Calculate check-sum for file
                            await Task.Run(() =>
                            {
                                const int BufferSize = 32768;
                                byte[] buffer = new byte[BufferSize];
                                Crc32 calculatedHash = new Crc32();

                                fileData = File.OpenRead(fileName);
                                int bytesRead = fileData.Read(buffer, 0, BufferSize);

                                while (bytesRead > 0)
                                {
                                    calculatedHash.Update(buffer, 0, bytesRead);
                                    bytesRead = fileData.Read(buffer, 0, BufferSize);
                                }

                                responseHash = calculatedHash.Value;
                                s_etagCache.TryAdd(fileName, responseHash);
                                fileData.Seek(0, SeekOrigin.Begin);

                                Program.Host.LogStatusMessage($"Cache [{responseHash}] added for file \"{fileName}\"");
                            });
                        }

                        if (PublishResponseContent(response, responseHash))
                        {
                            if ((object)fileData == null)
                                fileData = File.OpenRead(fileName);

                            response.Content = await Task.Run(() => new StreamContent(fileData));
                            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(pageName));
                        }
                        else
                        {
                            fileData?.Dispose();
                        }                       
                    }
                    else
                    {
                        response.StatusCode = HttpStatusCode.NotFound;
                    }
                    break;
            }

            return response;
        }

        private bool PublishResponseContent(HttpResponseMessage response, long responseHash)
        {
            if (!Program.Host.ClientCacheEnabled)
                return true;

            long requestHash;

            // See if client's version of cached resource is up to date
            foreach (EntityTagHeaderValue headerValue in Request.Headers.IfNoneMatch)
            {
                if (long.TryParse(headerValue.Tag?.Substring(1, headerValue.Tag.Length - 2), out requestHash) && responseHash == requestHash)
                {
                    response.StatusCode = HttpStatusCode.NotModified;
                    return false;
                }
            }

            response.Headers.CacheControl = new CacheControlHeaderValue
            {
                Public = true,
                MaxAge = new TimeSpan(31536000 * TimeSpan.TicksPerSecond)
            };

            response.Headers.ETag = new EntityTagHeaderValue($"\"{responseHash}\"");
            return true;
        }

        #region [ Sub-folder Handlers ]

        // Sub-folder request handler - depth 1
        [Route("{folder1}/{pageName}"), HttpGet]
        public Task<HttpResponseMessage> GetPage(string folder1, string pageName)
        {
            return GetPage($"{folder1}/{pageName}");
        }

        // Sub-folder post handler - depth 1
        [Route("{folder1}/{pageName}"), HttpPost]
        public Task<HttpResponseMessage> PostPage(string folder1, string pageName, dynamic postData)
        {
            return PostPage($"{folder1}/{pageName}", postData);
        }

        // Sub-folder request handler - depth 2
        [Route("{folder1}/{folder2}/{pageName}"), HttpGet]
        public Task<HttpResponseMessage> GetPage(string folder1, string folder2, string pageName)
        {
            return GetPage($"{folder1}/{folder2}/{pageName}");
        }

        // Sub-folder post handler - depth 2
        [Route("{folder1}/{folder2}/{pageName}"), HttpPost]
        public Task<HttpResponseMessage> PostPage(string folder1, string folder2, string pageName, dynamic postData)
        {
            return PostPage($"{folder1}/{folder2}/{pageName}", postData);
        }

        // Sub-folder request handler - depth 3
        [Route("{folder1}/{folder2}/{folder3}/{pageName}"), HttpGet]
        public Task<HttpResponseMessage> GetPage(string folder1, string folder2, string folder3, string pageName)
        {
            return GetPage($"{folder1}/{folder2}/{folder3}/{pageName}");
        }

        // Sub-folder post handler - depth 3
        [Route("{folder1}/{folder2}/{folder3}/{pageName}"), HttpPost]
        public Task<HttpResponseMessage> PostPage(string folder1, string folder2, string folder3, string pageName, dynamic postData)
        {
            return PostPage($"{folder1}/{folder2}/{folder3}/{pageName}", postData);
        }

        // Sub-folder request handler - depth 4
        [Route("{folder1}/{folder2}/{folder3}/{folder4}/{pageName}"), HttpGet]
        public Task<HttpResponseMessage> GetPage(string folder1, string folder2, string folder3, string folder4, string pageName)
        {
            return GetPage($"{folder1}/{folder2}/{folder3}/{folder4}/{pageName}");
        }

        // Sub-folder post handler - depth 4
        [Route("{folder1}/{folder2}/{folder3}/{folder4}/{pageName}"), HttpPost]
        public Task<HttpResponseMessage> PostPage(string folder1, string folder2, string folder3, string folder4, string pageName, dynamic postData)
        {
            return PostPage($"{folder1}/{folder2}/{folder3}/{folder4}/{pageName}", postData);
        }

        // Sub-folder request handler - depth 5
        [Route("{folder1}/{folder2}/{folder3}/{folder4}/{folder5}/{pageName}"), HttpGet]
        public Task<HttpResponseMessage> GetPage(string folder1, string folder2, string folder3, string folder4, string folder5, string pageName)
        {
            return GetPage($"{folder1}/{folder2}/{folder3}/{folder4}/{folder5}/{pageName}");
        }

        // Sub-folder post handler - depth 5
        [Route("{folder1}/{folder2}/{folder3}/{folder4}/{folder5}/{pageName}"), HttpPost]
        public Task<HttpResponseMessage> PostPage(string folder1, string folder2, string folder3, string folder4, string folder5, string pageName, dynamic postData)
        {
            return PostPage($"{folder1}/{folder2}/{folder3}/{folder4}/{folder5}/{pageName}", postData);
        }

        #endregion

        #endregion

        #region [ Static ]

        // Static Fields
        private static readonly ConcurrentDictionary<string, uint> s_etagCache;
        private static readonly FileSystemWatcher s_fileWatcher;

        // Static Constructor
        static WebPageController()
        {
            s_etagCache = new ConcurrentDictionary<string, uint>(StringComparer.InvariantCultureIgnoreCase);
            s_fileWatcher = new FileSystemWatcher(Program.Host.WebRootFolder)
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents = true
            };

            s_fileWatcher.Changed += s_fileWatcher_FileChange;
            s_fileWatcher.Deleted += s_fileWatcher_FileChange;
            s_fileWatcher.Renamed += s_fileWatcher_FileChange;

            // Initialize RazorView for target languages - this invokes static constructors which will pre-compile templates
            new RazorView<CSharp>();
            new RazorView<VisualBasic>();
        }

        private static void s_fileWatcher_FileChange(object sender, FileSystemEventArgs e)
        {
            uint responseHash;

            if (s_etagCache.TryRemove(e.FullPath, out responseHash))
                Program.Host.LogStatusMessage($"Cache [{responseHash}] cleared for file \"{e.FullPath}\"");
        }

        #endregion
    }
}