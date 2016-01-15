//******************************************************************************************************
//  ServiceClient.cs - Gbtc
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
//  09/15/2015 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using GSF;
using GSF.Console;
using GSF.Reflection;
using GSF.ServiceProcess;
using openMIC;

namespace openMICConsole
{
    /// <summary>
    /// Represents a remote console client for the openMIC service.
    /// </summary>
    public class ServiceClient : IDisposable
    {
        #region [ Members ]

        // Fields
        private ServiceConnection m_serviceConnection;
        private readonly ConsoleColor m_originalFgColor;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="ServiceClient"/> class.
        /// </summary>
        public ServiceClient()
        {
            m_serviceConnection = new ServiceConnection();
            m_serviceConnection.StatusMessage += m_serviceConnection_StatusMessage;
            m_serviceConnection.ServiceResponse += m_serviceConnection_ServiceResponse;

            // Save the color scheme.
            m_originalFgColor = Console.ForegroundColor;
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="ServiceClient"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~ServiceClient()
        {
            Dispose(false);
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases all the resources used by the <see cref="ServiceClient"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ServiceClient"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.

                    if (disposing)
                    {
                        if ((object)m_serviceConnection != null)
                        {
                            m_serviceConnection.StatusMessage -= m_serviceConnection_StatusMessage;
                            m_serviceConnection.ServiceResponse -= m_serviceConnection_ServiceResponse;
                            m_serviceConnection.Dispose();
                            m_serviceConnection = null;
                        }
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        /// <summary>
        /// Handles service start event.
        /// </summary>
        /// <param name="args">Service start arguments.</param>
        public void Start(string[] args)
        {
            string userInput = null;
            Arguments arguments = new Arguments(string.Join(" ", args));

            if (arguments.Exists("OrderedArg1") && arguments.Exists("restart"))
            {
                string serviceName = arguments["OrderedArg1"];

                // Attempt to access service controller for the specified Windows service
                ServiceController serviceController = ServiceController.GetServices().SingleOrDefault(svc => string.Compare(svc.ServiceName, serviceName, true) == 0);

                if (serviceController != null)
                {
                    try
                    {
                        if (serviceController.Status == ServiceControllerStatus.Running)
                        {
                            Console.WriteLine("Attempting to stop the {0} Windows service...", serviceName);

                            serviceController.Stop();

                            // Can't wait forever for service to stop, so we time-out after 20 seconds
                            serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(20.0D));

                            if (serviceController.Status == ServiceControllerStatus.Stopped)
                                Console.WriteLine("Successfully stopped the {0} Windows service.", serviceName);
                            else
                                Console.WriteLine("Failed to stop the {0} Windows service after trying for 20 seconds...", serviceName);

                            // Add an extra line for visual separation of service termination status
                            Console.WriteLine("");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to stop the {0} Windows service: {1}\r\n", serviceName, ex.Message);
                    }
                }

                // If the service failed to stop or it is installed as stand-alone debug application, we try to forcibly stop any remaining running instances
                try
                {
                    Process[] instances = Process.GetProcessesByName(serviceName);

                    if (instances.Length > 0)
                    {
                        int total = 0;
                        Console.WriteLine("Attempting to stop running instances of the {0}...", serviceName);

                        // Terminate all instances of service running on the local computer
                        foreach (Process process in instances)
                        {
                            process.Kill();
                            total++;
                        }

                        if (total > 0)
                            Console.WriteLine("Stopped {0} {1} instance{2}.", total, serviceName, total > 1 ? "s" : "");

                        // Add an extra line for visual separation of process termination status
                        Console.WriteLine("");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to terminate running instances of the {0}: {1}\r\n", serviceName, ex.Message);
                }

                // Attempt to restart Windows service...
                if (serviceController != null)
                {
                    try
                    {
                        // Refresh state in case service process was forcibly stopped
                        serviceController.Refresh();

                        if (serviceController.Status != ServiceControllerStatus.Running)
                            serviceController.Start();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to restart the {0} Windows service: {1}\r\n", serviceName, ex.Message);
                    }
                }
            }
            else
            {
                // Connect to service and send commands.
                while (!m_serviceConnection.Enabled)
                {
                    m_serviceConnection.Connect();

                    while (m_serviceConnection.Enabled && string.Compare(userInput, "Exit", true) != 0)
                    {
                        // Wait for a command from the user. 
                        userInput = Console.ReadLine();

                        // Write a blank line to the console.
                        Console.WriteLine();

                        if (!string.IsNullOrWhiteSpace(userInput))
                        {
                            // The user typed in a command and didn't just hit <ENTER>. 
                            switch (userInput.ToUpper())
                            {
                                case "CLS":
                                    // User wants to clear the console window. 
                                    Console.Clear();
                                    break;
                                case "EXIT":
                                    break;
                                default:
                                    // User wants to send a request to the service. 
                                    m_serviceConnection.SendCommand(userInput);

                                    if (string.Compare(userInput, "Help", true) == 0)
                                        DisplayHelp();

                                    break;
                            }
                        }
                    }
                }
            }
        }

        private void DisplayHelp()
        {
            StringBuilder help = new StringBuilder();

            help.AppendFormat("Commands supported by {0}:", AssemblyInfo.EntryAssembly.Name);
            help.AppendLine();
            help.AppendLine();
            help.Append("Command".PadRight(20));
            help.Append(" ");
            help.Append("Description".PadRight(55));
            help.AppendLine();
            help.Append(new string('-', 20));
            help.Append(" ");
            help.Append(new string('-', 55));
            help.AppendLine();
            help.Append("Cls".PadRight(20));
            help.Append(" ");
            help.Append("Clears this console screen".PadRight(55));
            help.AppendLine();
            help.Append("Exit".PadRight(20));
            help.Append(" ");
            help.Append("Exits this console screen".PadRight(55));
            help.AppendLine();
            help.AppendLine();
            help.AppendLine();

            Console.Write(help.ToString());
        }

        // Client helper service update reception handler.
        private void m_serviceConnection_StatusMessage(object sender, EventArgs<UpdateType, string> e)
        {
            // Output status updates from the service to the console window.
            switch (e.Argument1)
            {
                case UpdateType.Alarm:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case UpdateType.Information:
                    Console.ForegroundColor = m_originalFgColor;
                    break;
                case UpdateType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
            }

            Console.Write(e.Argument2);
            Console.ForegroundColor = m_originalFgColor;
        }

        // Client helper service response reception handler.
        private void m_serviceConnection_ServiceResponse(object sender, EventArgs<ServiceResponse, string, bool> e)
        {
            string message = e.Argument1.Message;
            string sourceCommand = e.Argument2;
            bool responseSuccess = e.Argument3;

            if (responseSuccess)
            {
                if (string.IsNullOrWhiteSpace(message))
                    Console.Write("{0} command processed successfully.\r\n\r\n", sourceCommand);
                else
                    Console.Write("{0}\r\n\r\n", message);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;

                if (string.IsNullOrWhiteSpace(message))
                    Console.Write("{0} failure.\r\n\r\n", sourceCommand);
                else
                    Console.Write("{0} failure: {1}\r\n\r\n", sourceCommand, message);

                Console.ForegroundColor = m_originalFgColor;
            }
        }

        #endregion
    }
}