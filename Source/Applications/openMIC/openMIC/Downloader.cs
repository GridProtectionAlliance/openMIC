//******************************************************************************************************
//  Downloader.cs - Gbtc
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
//  12/08/2015 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System.ComponentModel;
using System.IO;
using DotRas;
using GSF.Configuration;
using GSF.TimeSeries.Adapters;

namespace openMIC
{
    [Description("Downloader: Implements remote file download capabilities")]
    [EditorBrowsable(EditorBrowsableState.Advanced)] // Normally defined as an input device protocol
    public class Downloader : FacileActionAdapterBase
    {
        #region [ Members ]

        // Nested Types

        // Constants

        // Delegates

        // Events

        // Fields
        private readonly RasDialer m_rasDialer;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        public Downloader()
        {
            m_rasDialer = new RasDialer();
            m_rasDialer.Error += m_rasDialer_Error;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets connection host name or IP for transport.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines connection host name or IP for transport.")]
        public string ConnectionHostName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets connection host user name for transport.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines connection host user name for transport.")]
        public string ConnectionUserName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets connection password for transport.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines connection password for transport.")]
        public string ConnectionPassword
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets flag that determines if this connection will use dial-up.
        /// </summary>
        [ConnectionStringParameter,
        Description("Determines if this connection will use dial-up.")]
        public bool useDialUp
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets dial-up phone number.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines dial-up phone number.")]
        public string DialUpNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets dial-up user name.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines dial-up user name.")]
        public string DialUpUserName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets dial-up password.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines dial-up password.")]
        public string DialUpPassword
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets maximum retries for a dial-up connection.
        /// </summary>
        [ConnectionStringParameter,
        DefaultValue(3),
        Description("Defines maximum retries for a dial-up connection.")]
        public int DialUpRetries
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets timeout for a dial-up connection.
        /// </summary>
        [ConnectionStringParameter,
        DefaultValue(90),
        Description("Defines timeout for a dial-up connection.")]
        public int DialUpTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets download schedule.
        /// </summary>
        [ConnectionStringParameter,
        DefaultValue("* * * * *"),
        Description("Defines download schedule.")]
        public string Schedule
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the flag indicating if this adapter supports temporal processing.
        /// </summary>
        public override bool SupportsTemporalProcessing => false;

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="Downloader"/> object and optionally releases the managed resources.
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
                        if ((object)m_rasDialer != null)
                        {
                            m_rasDialer.Error -= m_rasDialer_Error;
                            m_rasDialer.Dispose();
                        }
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
                }
            }
        }

        /// <summary>
        /// Initializes <see cref="Downloader"/>.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            ConnectionStringParser<ConnectionStringParameterAttribute> parser = new ConnectionStringParser<ConnectionStringParameterAttribute>();
            parser.ParseConnectionString(ConnectionString, this);
        }

        /// <summary>
        /// Gets a short one-line status of this adapter.
        /// </summary>
        /// <param name="maxLength">Maximum number of available characters for display.</param>
        /// <returns>
        /// A short one-line summary of the current status of this adapter.
        /// </returns>
        public override string GetShortStatus(int maxLength)
        {
            if (!Enabled)
                return "Downloading for is paused...";

            return "Downloading is enabled...";
        }

        private void m_rasDialer_Error(object sender, ErrorEventArgs e)
        {
            OnProcessException(e.GetException());
        }

        #endregion
    }
}
