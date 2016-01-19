//******************************************************************************************************
//  ServiceHost.cs - Gbtc
//
//  Copyright © 2015, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  09/02/2009 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using GSF.TimeSeries;
using GSF;
using GSF.Configuration;
using GSF.IO;
using Microsoft.Owin.Hosting;
using GSF.ServiceProcess;

namespace openMIC
{
    public class ServiceHost : ServiceHostBase
    {
        #region [ Members ]

        // Events

        /// <summary>
        /// Raised when there is a new status message reported to service.
        /// </summary>
        public event EventHandler<EventArgs<string, UpdateType>> UpdatedStatus;

        /// <summary>
        /// Raised when there is a new exception logged to service.
        /// </summary>
        public event EventHandler<EventArgs<Exception>> LoggedException;

        // Fields
        private IDisposable m_webAppHost;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new service host for openMIC application.
        /// </summary>
        public ServiceHost()
        {
            ServiceName = "openMIC";
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets configured web root folder for the application.
        /// </summary>
        public string WebRootFolder
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets configured default web page for the application.
        /// </summary>
        public string DefaultWebPage
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the model used for the application.
        /// </summary>
        public object Model
        {
            get;
            private set;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ServiceHost"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    if (disposing)
                    {
                        if ((object)m_webAppHost != null)
                            m_webAppHost.Dispose();
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
                }
            }
        }

        protected override void ServiceStartingHandler(object sender, EventArgs<string[]> e)
        {
            // Handle base class service starting procedures
            base.ServiceStartingHandler(sender, e);

            // Make sure openMIC specific default service settings exist
            CategorizedSettingsElementCollection systemSettings = ConfigurationFile.Current.Settings["systemSettings"];

            systemSettings.Add("CompanyName", "Grid Protection Alliance", "The name of the company who owns this instance of the openMIC.");
            systemSettings.Add("CompanyAcronym", "GPA", "The acronym representing the company who owns this instance of the openMIC.");
            systemSettings.Add("WebHostURL", "http://localhost:8989", "The web hosting URL for remote system management.");
            systemSettings.Add("WebRootFolder", "wwwroot", "The default root for the hosted web server files. Location will be relative to install folder if full path is not specified.");
            systemSettings.Add("DefaultWebPage", "Index.cshtml", "The default web page for the hosted web server.");

            // Get configured web settings
            WebRootFolder = FilePath.GetAbsolutePath(systemSettings["WebRootFolder"].Value);
            DefaultWebPage = systemSettings["DefaultWebPage"].Value;

            // TODO: Define needed application model properties
            Model = new
            {
                CompanyName = systemSettings["CompanyName"].Value,
                CompanyAcronym = systemSettings["CompanyAcronym"].Value,
                ApplicationName = "openMIC"
            };

            // Create new web application hosting environment
            m_webAppHost = WebApp.Start<Startup>(systemSettings["WebHostURL"].Value);

            ServiceHelper.UpdatedStatus += UpdatedStatusHandler;
            ServiceHelper.LoggedException += LoggedExceptionHandler;
        }

        protected override void ServiceStoppingHandler(object sender, EventArgs e)
        {
            base.ServiceStoppingHandler(sender, e);

            ServiceHelper.UpdatedStatus -= UpdatedStatusHandler;
            ServiceHelper.LoggedException -= LoggedExceptionHandler;
        }

        /// <summary>
        /// Logs a status message to connected clients.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="type">Type of message to log.</param>
        public void LogStatusMessage(string message, UpdateType type = UpdateType.Information)
        {
            DisplayStatusMessage(message, type);
        }

        /// <summary>
        /// Sends a command request to the service.
        /// </summary>
        /// <param name="userInput">Request string.</param>
        public void SendRequest(string userInput)
        {
            ClientRequest request = ClientRequest.Parse(userInput);

            if ((object)request != null)
            {
                ClientRequestHandler requestHandler = ServiceHelper.FindClientRequestHandler(request.Command);

                if ((object)requestHandler != null)
                    requestHandler.HandlerMethod(new ClientRequestInfo(new ClientInfo(), request));
                else
                    DisplayStatusMessage(string.Format("Command \"{0}\" is not supported\r\n\r\n", request.Command), UpdateType.Alarm);
            }
        }

        private void UpdatedStatusHandler(object sender, EventArgs<string, UpdateType> e)
        {
            if ((object)UpdatedStatus != null)
                UpdatedStatus(sender, new EventArgs<string, UpdateType>(e.Argument1, e.Argument2));
        }

        private void LoggedExceptionHandler(object sender, EventArgs<Exception> e)
        {
            if ((object)LoggedException != null)
                LoggedException(sender, new EventArgs<Exception>(e.Argument));
        }

        #endregion
    }
}
