//******************************************************************************************************
//  DebugHost.cs - Gbtc
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
//  05/15/2009 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System.ComponentModel;
using GSF.TimeSeries;

namespace openMIC
{
    public class DebugHost : DebugHostBase
    {
        public DebugHost(ServiceHost host)
        {
            ServiceHost = host;
            InitializeComponent();
        }

        protected override string ServiceClientName => "openMICConsole.exe";

        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(DebugHost));
            
            SuspendLayout();

            // 
            // DebugHost
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            ClientSize = new System.Drawing.Size(344, 73);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "DebugHost";
            
            ResumeLayout(false);
        }
    }
}