//******************************************************************************************************
//  DebugHostBase.cs - Gbtc
//
//  Copyright © 2015, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  05/15/2009 - J. Ritchie Carroll
//       Generated original version of source code.
//  12/20/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

using System;
using System.Diagnostics;
using System.Windows.Forms;
using GSF.IO;
using GSF.Reflection;

namespace openMIC
{
    /// <summary>
    /// Windows form application used to host the time-series framework service.
    /// </summary>
    public partial class DebugHost
    {
        #region [ Members ]

        // Fields
        private string m_productName;
        private Process m_remoteConsole;
        private Process m_remoteManager;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="DebugHost"/> windows form.
        /// </summary>
        public DebugHost()
        {
            InitializeComponent();
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Invoked when the debug host is loading. By default this launches the remote service client.
        /// </summary>
        protected virtual void DebugHostLoading()
        {
            // Start remote console session
            const string serviceClientName = "openMICConsole.exe";
            const string serviceManager = "openMICManager.exe";

            if (!string.IsNullOrWhiteSpace(serviceClientName))
                m_remoteConsole = Process.Start(FilePath.GetAbsolutePath(serviceClientName));

            if (!string.IsNullOrWhiteSpace(serviceManager))
                m_remoteManager = Process.Start(FilePath.GetAbsolutePath(serviceManager));
        }

        /// <summary>
        /// Invoked when the debug host is unloading. By default this shuts down the remote service client.
        /// </summary>
        protected virtual void DebugHostUnloading()
        {
            // Close remote console session
            if (m_remoteConsole != null && !m_remoteConsole.HasExited)
                m_remoteConsole.Kill();

            // Close remote manager session
            if (m_remoteManager != null && !m_remoteManager.HasExited)
                m_remoteManager.Kill();
        }

        private void DebugHost_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                // Call user overridable debug host loading method
                DebugHostLoading();

                // Initialize text.
                m_productName = AssemblyInfo.EntryAssembly.Title;
                Text = string.Format(Text, m_productName);
                m_notifyIcon.Text = string.Format(m_notifyIcon.Text, m_productName);
                m_labelNotice.Text = string.Format(m_labelNotice.Text, m_productName);
                m_exitToolStripMenuItem.Text = string.Format(m_exitToolStripMenuItem.Text, m_productName);

                // Minimize the window.
                WindowState = FormWindowState.Minimized;

                // Start the windows service.
                m_serviceHost.StartDebugging(Environment.CommandLine.Split(' '));
            }
        }

        private void DebugHost_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!DesignMode)
            {
                if (MessageBox.Show(string.Format("Are you sure you want to stop {0} service? ", m_productName),
                    "Stop Service", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Stop the windows service.
                    m_serviceHost.StopDebugging();

                    // Call user overridable debug host unloading method
                    DebugHostUnloading();
                }
                else
                {
                    // Stop the application from exiting.
                    e.Cancel = true;
                }
            }
        }

        private void DebugHost_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                // Don't show the window in taskbar when minimized.
                ShowInTaskbar = false;
            }
        }

        private void m_showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show the window in taskbar the in normal mode (visible).
            ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
        }

        private void m_exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Close this window which will cause the application to exit.
            Close();
        }

        #endregion
    }
}