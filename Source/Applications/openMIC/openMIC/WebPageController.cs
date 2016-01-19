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

using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using GSF.IO;

namespace openMIC
{
    /// <summary>
    /// Defines a mini-web server with Razor support using the self-hosted API controller.
    /// </summary>
    public class WebPageController : ApiController
    {
        // Default page request handler
        [Route, HttpGet]
        public HttpResponseMessage GetPage()
        {
            return GetPage(Program.Host.DefaultWebPage);
        }

        // Common request handler
        [Route("{pageName}"), HttpGet]
        public HttpResponseMessage GetPage(string pageName)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            string fileExtension = FilePath.GetExtension(pageName).ToLowerInvariant();

            switch (fileExtension)
            {
                case ".cshtml":
                    response.Content = new StringContent(new RazorView<CSharp>(pageName, Program.Host.Model).Run(), Encoding.UTF8, "text/html");
                    break;
                case ".vbhtml":
                    response.Content = new StringContent(new RazorView<VisualBasic>(pageName, Program.Host.Model).Run(), Encoding.UTF8, "text/html");
                    break;
                default:
                    string fileName = FilePath.GetAbsolutePath($"{Program.Host.WebRootFolder}\\{pageName}");

                    if (File.Exists(fileName))
                    {
                        response.Content = new StreamContent(File.OpenRead(fileName));
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(pageName));
                    }
                    else
                    {
                        response.StatusCode = HttpStatusCode.NotFound;
                    }
                    break;
            }

            return response;
        }

        // Sub folder request handler - depth 1
        [Route("{folder1}/{pageName}"), HttpGet]
        public HttpResponseMessage GetPage(string folder1, string pageName)
        {
            return GetPage($"{folder1}/{pageName}");
        }

        // Sub folder request handler - depth 2
        [Route("{folder1}/{folder2}/{pageName}"), HttpGet]
        public HttpResponseMessage GetPage(string folder1, string folder2, string pageName)
        {
            return GetPage($"{folder1}/{folder2}/{pageName}");
        }

        // Sub folder request handler - depth 3
        [Route("{folder1}/{folder2}/{folder3}/{pageName}"), HttpGet]
        public HttpResponseMessage GetPage(string folder1, string folder2, string folder3, string pageName)
        {
            return GetPage($"{folder1}/{folder2}/{folder3}/{pageName}");
        }

        // Sub folder request handler - depth 4
        [Route("{folder1}/{folder2}/{folder3}/{folder4}/{pageName}"), HttpGet]
        public HttpResponseMessage GetPage(string folder1, string folder2, string folder3, string folder4, string pageName)
        {
            return GetPage($"{folder1}/{folder2}/{folder3}/{folder4}/{pageName}");
        }

        // Sub folder request handler - depth 5
        [Route("{folder1}/{folder2}/{folder3}/{folder4}/{folder5}/{pageName}"), HttpGet]
        public HttpResponseMessage GetPage(string folder1, string folder2, string folder3, string folder4, string folder5, string pageName)
        {
            return GetPage($"{folder1}/{folder2}/{folder3}/{folder4}/{folder5}/{pageName}");
        }
    }
}
