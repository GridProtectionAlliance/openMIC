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

using GSF;
using GSF.ComponentModel;
using GSF.Configuration;
using GSF.Diagnostics;
using GSF.IO;
using GSF.Security;
using GSF.Security.Model;
using GSF.ServiceProcess;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using GSF.Web.Hosting;
using GSF.Web.Model;
using GSF.Web.Model.Handlers;
using GSF.Web.Security;
using GSF.Web.Shared;
using GSF.Web.Shared.Model;
using Microsoft.Ajax.Utilities;
using Microsoft.Owin.Hosting;
using openMIC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.WebUtility;

namespace openMIC;

public class ServiceHost : ServiceHostBase
{
    #region [ Members ]

    // Constants
    private const int DefaultMaximumDiagnosticLogSize = 10;
    private const string DefaultMinifyJavascriptExclusionExpression = @"^/?Scripts/SectionMapBuilder\.js$";

    // Events

    /// <summary>
    /// Raised when there is a new status message reported to service.
    /// </summary>
    public event EventHandler<EventArgs<Guid, string, UpdateType>> UpdatedStatus;

    /// <summary>
    /// Raised when there is a new exception logged to service.
    /// </summary>
    public event EventHandler<EventArgs<Exception>> LoggedException;

    // Fields
    private IDisposable m_webAppHost;
    private long m_currentPoolTarget;
    private bool m_serviceStopping;
    private bool m_disposed;

    #endregion

    #region [ Constructors ]

    /// <summary>
    /// Creates a new <see cref="ServiceHost"/> instance.
    /// </summary>
    public ServiceHost()
    {
        ServiceName = "openMIC";

        try
        {
            // Assign default minification exclusion early (well before web server static initialization)
            CategorizedSettingsElementCollection systemSettings = ConfigurationFile.Current.Settings["systemSettings"];
            systemSettings.Add("MinifyJavascriptExclusionExpression", DefaultMinifyJavascriptExclusionExpression, "Defines the regular expression that will exclude Javascript files from being minified. Empty value will target all files for minification.");

            if (string.IsNullOrWhiteSpace(systemSettings["MinifyJavascriptExclusionExpression"].Value))
                systemSettings["MinifyJavascriptExclusionExpression"].Value = DefaultMinifyJavascriptExclusionExpression;
        }
        catch (Exception ex)
        {
            Logger.SwallowException(ex);
        }
    }

    #endregion

    #region [ Properties ]

    /// <summary>
    /// Gets the configured default web page for the application.
    /// </summary>
    public string DefaultWebPage { get; private set; }

    /// <summary>
    /// Gets the model used for the application.
    /// </summary>
    public AppModel Model { get; private set; }

    /// <summary>
    /// Gets the list of <see cref="Downloader"/> adapters that were loaded into the input adapter collection.
    /// </summary>
    public Downloader[] Downloaders
    {
        get
        {
            lock (InputAdapters)
                return InputAdapters.OfType<Downloader>().ToArray();
        }
    }

    /// <summary>
    /// Gets current performance statistics.
    /// </summary>
    public string PerformanceStatistics => ServiceHelper?.PerformanceMonitor?.Status;

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="ServiceHost"/> object and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (m_disposed)
            return;

        try
        {
            if (disposing)
            {
                m_webAppHost?.Dispose();
            }
        }
        finally
        {
            m_disposed = true;       // Prevent duplicate dispose.
            base.Dispose(disposing); // Call base class Dispose().
        }
    }

    /// <summary>
    /// Event handler for service starting operations.
    /// </summary>
    /// <param name="sender">Event source.</param>
    /// <param name="e">Event arguments containing command line arguments passed into service at startup.</param>
    protected override void ServiceStartingHandler(object sender, EventArgs<string[]> e)
    {
        // Handle base class service starting procedures
        base.ServiceStartingHandler(sender, e);

        // Make sure openMIC specific default service settings exist
        CategorizedSettingsElementCollection systemSettings = ConfigurationFile.Current.Settings["systemSettings"];
        CategorizedSettingsElementCollection securityProvider = ConfigurationFile.Current.Settings["securityProvider"];

        // Define set of default anonymous web resources for this site
        const string BaseAnonymousApiExpression = "^/api/";
        const string AnonymousApiExpression = $"{BaseAnonymousApiExpression}(?!ModbusConfig)"; // Modbus config API should not be anonymous
        const string DefaultAnonymousResourceExpression = $"^/@|^/Scripts/|^/Content/|^/Images/|^/fonts/|{AnonymousApiExpression}|^/favicon.ico$";

        systemSettings.Add("CompanyName", "Grid Protection Alliance", "The name of the company who owns this instance of the openMIC.");
        systemSettings.Add("CompanyAcronym", "GPA", "The acronym representing the company who owns this instance of the openMIC.");
        systemSettings.Add("DiagnosticLogPath", FilePath.GetAbsolutePath(""), "Path for diagnostic logs.");
        systemSettings.Add("MaximumDiagnosticLogSize", DefaultMaximumDiagnosticLogSize, "The combined maximum size for the diagnostic logs in whole Megabytes; curtailment happens hourly. Set to zero for no limit.");
        systemSettings.Add("WebHostURL", "http://+:8089", "The web hosting URL for remote system management.");
        systemSettings.Add("WebRootPath", "wwwroot", "The root path for the hosted web server files. Location will be relative to install folder if full path is not specified.");
        systemSettings.Add("DefaultWebPage", "Index.cshtml", "The default web page for the hosted web server.");
        systemSettings.Add("DateFormat", "MM/dd/yyyy", "The default date format to use when rendering timestamps.");
        systemSettings.Add("TimeFormat", "HH:mm:ss.fff", "The default time format to use when rendering timestamps.");
        systemSettings.Add("BootstrapTheme", "Content/bootstrap.min.css", "Path to Bootstrap CSS to use for rendering styles.");
        systemSettings.Add("SubscriptionConnectionString", "server=localhost:6195; interface=0.0.0.0", "Connection string for data subscriptions to openMIC server.");
        systemSettings.Add("AuthenticationSchemes", AuthenticationOptions.DefaultAuthenticationSchemes, "Comma separated list of authentication schemes to use for clients accessing the hosted web server, e.g., Basic or NTLM.");
        systemSettings.Add("AuthFailureRedirectResourceExpression", AuthenticationOptions.DefaultAuthFailureRedirectResourceExpression, "Expression that will match paths for the resources on the web server that should redirect to the LoginPage when authentication fails.");
        systemSettings.Add("AnonymousResourceExpression", DefaultAnonymousResourceExpression, "Expression that will match paths for the resources on the web server that can be provided without checking credentials.");
        systemSettings.Add("AuthenticationToken", SessionHandler.DefaultAuthenticationToken, "Defines the token used for identifying the authentication token in cookie headers.");
        systemSettings.Add("SessionToken", SessionHandler.DefaultSessionToken, "Defines the token used for identifying the session ID in cookie headers.");
        systemSettings.Add("RequestVerificationToken", AuthenticationOptions.DefaultRequestVerificationToken, "Defines the token used for anti-forgery verification in HTTP request headers.");
        systemSettings.Add("LoginPage", AuthenticationOptions.DefaultLoginPage, "Defines the login page used for redirects on authentication failure. Expects forward slash prefix.");
        systemSettings.Add("AuthTestPage", AuthenticationOptions.DefaultAuthTestPage, "Defines the page name for the web server to test if a user is authenticated. Expects forward slash prefix.");
        systemSettings.Add("Realm", "", "Case-sensitive identifier that defines the protection space for the web based authentication and is used to indicate a scope of protection.");
        systemSettings.Add("DefaultCorsOrigins", "", "Comma-separated list of allowed origins (including http:// prefix) that define the default CORS policy. Use '*' to allow all or empty string to disable CORS.");
        systemSettings.Add("DefaultCorsHeaders", "*", "Comma-separated list of supported headers that define the default CORS policy. Use '*' to allow all or empty string to allow none.");
        systemSettings.Add("DefaultCorsMethods", "*", "Comma-separated list of supported methods that define the default CORS policy. Use '*' to allow all or empty string to allow none.");
        systemSettings.Add("DefaultCorsSupportsCredentials", true, "Boolean flag for the default CORS policy indicating whether the resource supports user credentials in the request.");

        systemSettings.Add("DefaultDialUpRetries", 3, "Default dial-up connection retries.");
        systemSettings.Add("DefaultDialUpTimeout", 90, "Default dial-up connection timeout.");
        systemSettings.Add("DefaultFTPUserName", "anonymous", "Default FTP user name to use for device connections.");
        systemSettings.Add("DefaultFTPPassword", "anonymous", "Default FTP password to use for device connections.");
        systemSettings.Add("DefaultRemotePath", "/", "Default remote FTP path to use for device connections.");
        systemSettings.Add("DefaultLocalPath", "", "Default local path to use for file downloads.");
        systemSettings.Add("MaxRemoteFileAge", "30", "Maximum remote file age, in days, to apply for downloads when limit is enabled.");
        systemSettings.Add("MaxLocalFileAge", "365", "Maximum local file age, in days, to apply for downloads when limit is enabled.");
        systemSettings.Add("SmtpServer", "localhost", "The SMTP relay server from which to send e-mails.");
        systemSettings.Add("FromAddress", "openmic@gridprotectionalliance.org", "The from address for e-mails.");
        systemSettings.Add("SmtpUserName", "", "Username to authenticate to the SMTP server, if any.");
        systemSettings.Add("SmtpPassword", "", "Password to authenticate to the SMTP server, if any.");
        systemSettings.Add("PoolMachines", "", "Comma separated list of openMIC pooled load-balancing machines. Should be blank when UseRemoteScheduler is true.");
        systemSettings.Add("UseHostAsPoolMachine", false, "Flag that determines if host machine should be in load-balanced pool. Set to true to force use the host machine as a pool machine.");
        systemSettings.Add("UseRemoteScheduler", false, "Flag that determines if scheduling is handled locally or managed by a remote scheduling system");
        systemSettings.Add("SystemName", "", "Name of system that will be prefixed to system level tags, when defined. Value should follow tag naming conventions, e.g., no spaces and all upper case.");
        systemSettings.Add("HideRestartButton", false, "Flag that determines if restart button should be displayed on the home page. Set to false for clustered environments.");

        // Ensure "^/api/(?!ModbusConfig)" exists in AnonymousResourceExpression
        string anonymousResourceExpression = systemSettings["AnonymousResourceExpression"].Value;

        if (anonymousResourceExpression.Contains($"{BaseAnonymousApiExpression}|"))
        {
            anonymousResourceExpression = anonymousResourceExpression.Replace($"{BaseAnonymousApiExpression}|", $"{AnonymousApiExpression}|");
            systemSettings["AnonymousResourceExpression"].Update(anonymousResourceExpression);
            ConfigurationFile.Current.Save();
        }
        
        if (!anonymousResourceExpression.ToLowerInvariant().Contains(AnonymousApiExpression.ToLowerInvariant()))
        {
            systemSettings["AnonymousResourceExpression"].Update($"{AnonymousApiExpression}|{anonymousResourceExpression}");
            ConfigurationFile.Current.Save();
        }

        DefaultWebPage = systemSettings["DefaultWebPage"].Value;

        // Correct config error from older installations
        if (systemSettings["TimeFormat"].Value.Contains("HH:mm.ss.fff"))
        {
            systemSettings["TimeFormat"].Value = "HH:mm:ss.fff";
            ConfigurationFile.Current.Save();
        }

        Model = new AppModel();
        Model.Global.CompanyName = systemSettings["CompanyName"].Value;
        Model.Global.CompanyAcronym = systemSettings["CompanyAcronym"].Value;
        Model.Global.NodeID = Guid.Parse(systemSettings["NodeID"].Value);
        Model.Global.SubscriptionConnectionString = systemSettings["SubscriptionConnectionString"].Value;
        Model.Global.ApplicationName = "openMIC";
        Model.Global.ApplicationDescription = "open Meter Information Collection System";
        Model.Global.ApplicationKeywords = "open source, utility, software, meter, interrogation";
        Model.Global.DateFormat = systemSettings["DateFormat"].Value;
        Model.Global.TimeFormat = systemSettings["TimeFormat"].Value;
        Model.Global.DateTimeFormat = $"{Model.Global.DateFormat} {Model.Global.TimeFormat}";
        Model.Global.PasswordRequirementsRegex = securityProvider["PasswordRequirementsRegex"].Value;
        Model.Global.PasswordRequirementsError = securityProvider["PasswordRequirementsError"].Value;
        Model.Global.BootstrapTheme = systemSettings["BootstrapTheme"].Value;
        Model.Global.DefaultDialUpRetries = int.Parse(systemSettings["DefaultDialUpRetries"].Value);
        Model.Global.DefaultDialUpTimeout = int.Parse(systemSettings["DefaultDialUpTimeout"].Value);
        Model.Global.DefaultFTPUserName = systemSettings["DefaultFTPUserName"].Value;
        Model.Global.DefaultFTPPassword = systemSettings["DefaultFTPPassword"].Value;
        Model.Global.DefaultRemotePath = systemSettings["DefaultRemotePath"].Value;
        Model.Global.DefaultLocalPath = FilePath.GetAbsolutePath(systemSettings["DefaultLocalPath"].Value);
        Model.Global.MaxRemoteFileAge = int.Parse(systemSettings["MaxRemoteFileAge"].Value);
        Model.Global.MaxLocalFileAge = int.Parse(systemSettings["MaxLocalFileAge"].Value);
        Model.Global.DefaultAppPath = FilePath.GetAbsolutePath("");
        Model.Global.SmtpServer = systemSettings["SmtpServer"].Value;
        Model.Global.FromAddress = systemSettings["FromAddress"].Value;
        Model.Global.SmtpUserName = systemSettings["SmtpUserName"].Value;
        Model.Global.SmtpPassword = systemSettings["SmtpPassword"].Value;
        Model.Global.WebRootPath = FilePath.GetAbsolutePath(systemSettings["WebRootPath"].Value);
        Model.Global.DefaultCorsOrigins = systemSettings["DefaultCorsOrigins"].Value;
        Model.Global.DefaultCorsHeaders = systemSettings["DefaultCorsHeaders"].Value;
        Model.Global.DefaultCorsMethods = systemSettings["DefaultCorsMethods"].Value;
        Model.Global.DefaultCorsSupportsCredentials = systemSettings["DefaultCorsSupportsCredentials"].ValueAsBoolean(true);
        Model.Global.SystemName = systemSettings["SystemName"].Value;
        Model.Global.HideRestartButton = systemSettings["HideRestartButton"].ValueAsBoolean(false);

        // Setup pooled machine configuration
        string poolMachines = systemSettings["PoolMachines"].Value;

        if (!string.IsNullOrWhiteSpace(poolMachines))
        {
            // Add configured pool machines making sure to also add local machine
            HashSet<string> poolMachineList = new(poolMachines.Split(','));

            if (systemSettings["UseHostAsPoolMachine"].ValueAs(false))
                poolMachineList.Add("localhost");

            Model.Global.PoolMachines = poolMachineList.ToArray();

            // An openMIC instance with defined pool machines is considered the schedule master
            Model.Global.UseRemoteScheduler = false;
        }
        else
        {
            // When UseRemoteScheduler is true, local task scheduling will be ignored and the openMIC
            // instance will be considered a subordinate in a pool of machines. When the setting is
            // false, the openMIC instance will be considered independent
            Model.Global.UseRemoteScheduler = systemSettings["UseRemoteScheduler"].ValueAs(false);
        }

        // Register a symbolic reference to global settings for use by default value expressions
        ValueExpressionParser.DefaultTypeRegistry.RegisterSymbol("Global", Model.Global);

        // Get web host URL and parse URI components
        string webHostURL = systemSettings["WebHostURL"].Value;
        Model.Global.WebHostUri = new Uri(webHostURL.Replace("+", "localhost"));

        // Parse configured authentication schemes
        if (!Enum.TryParse(systemSettings["AuthenticationSchemes"].ValueAs(AuthenticationOptions.DefaultAuthenticationSchemes.ToString()), true, out AuthenticationSchemes authenticationSchemes))
            authenticationSchemes = AuthenticationOptions.DefaultAuthenticationSchemes;

        // Initialize web startup configuration
        Startup.AuthenticationOptions.AuthenticationSchemes = authenticationSchemes;
        Startup.AuthenticationOptions.AuthFailureRedirectResourceExpression = systemSettings["AuthFailureRedirectResourceExpression"].ValueAs(AuthenticationOptions.DefaultAuthFailureRedirectResourceExpression);
        Startup.AuthenticationOptions.AnonymousResourceExpression = systemSettings["AnonymousResourceExpression"].ValueAs(DefaultAnonymousResourceExpression);
        Startup.AuthenticationOptions.AuthenticationToken = systemSettings["AuthenticationToken"].ValueAs(SessionHandler.DefaultAuthenticationToken);
        Startup.AuthenticationOptions.SessionToken = systemSettings["SessionToken"].ValueAs(SessionHandler.DefaultSessionToken);
        Startup.AuthenticationOptions.RequestVerificationToken = systemSettings["RequestVerificationToken"].ValueAs(AuthenticationOptions.DefaultRequestVerificationToken);
        Startup.AuthenticationOptions.LoginPage = systemSettings["LoginPage"].ValueAs(AuthenticationOptions.DefaultLoginPage);
        Startup.AuthenticationOptions.AuthTestPage = systemSettings["AuthTestPage"].ValueAs(AuthenticationOptions.DefaultAuthTestPage);
        Startup.AuthenticationOptions.Realm = systemSettings["Realm"].ValueAs("");
        Startup.AuthenticationOptions.LoginHeader = $"<h3><img src=\"/Images/{Model.Global.ApplicationName}.png\"/> {Model.Global.ApplicationName}</h3>";

        // Validate that configured authentication test page does not evaluate as an anonymous resource nor a authentication failure redirection resource
        string authTestPage = Startup.AuthenticationOptions.AuthTestPage;

        if (Startup.AuthenticationOptions.IsAnonymousResource(authTestPage))
            throw new SecurityException($"The configured authentication test page \"{authTestPage}\" evaluates as an anonymous resource. Modify \"AnonymousResourceExpression\" setting so that authorization test page is not a match.");

        if (Startup.AuthenticationOptions.IsAuthFailureRedirectResource(authTestPage))
            throw new SecurityException($"The configured authentication test page \"{authTestPage}\" evaluates as an authentication failure redirection resource. Modify \"AuthFailureRedirectResourceExpression\" setting so that authorization test page is not a match.");

        if (Startup.AuthenticationOptions.AuthenticationToken == Startup.AuthenticationOptions.SessionToken)
            throw new InvalidOperationException("Authentication token must be different from session token in order to differentiate the cookie values in the HTTP headers.");

        ServiceHelper.UpdatedStatus += UpdatedStatusHandler;
        ServiceHelper.LoggedException += LoggedExceptionHandler;

        // Define exception logger for CSV downloader
        CsvDownloadHandler.LogExceptionHandler = LogException;

        Thread startWebServer = new(() =>
        {
            try
            {
                // Attach to default web server events
                WebServer webServer = WebServer.Default;
                webServer.StatusMessage += WebServer_StatusMessage;
                webServer.ExecutionException += LoggedExceptionHandler;

                // Define types for Razor pages - self-hosted web service does not use view controllers so
                // we must define configuration types for all paged view model based Razor views here:
                webServer.PagedViewModelTypes.TryAdd("Devices.cshtml", new Tuple<Type, Type>(typeof(Device), typeof(DataHub)));
                webServer.PagedViewModelTypes.TryAdd("Companies.cshtml", new Tuple<Type, Type>(typeof(Company), typeof(SharedHub)));
                webServer.PagedViewModelTypes.TryAdd("Vendors.cshtml", new Tuple<Type, Type>(typeof(Vendor), typeof(SharedHub)));
                webServer.PagedViewModelTypes.TryAdd("VendorDevices.cshtml", new Tuple<Type, Type>(typeof(VendorDevice), typeof(SharedHub)));
                webServer.PagedViewModelTypes.TryAdd("Users.cshtml", new Tuple<Type, Type>(typeof(UserAccount), typeof(SecurityHub)));
                webServer.PagedViewModelTypes.TryAdd("Groups.cshtml", new Tuple<Type, Type>(typeof(SecurityGroup), typeof(SecurityHub)));
                webServer.PagedViewModelTypes.TryAdd("ConnectionProfiles.cshtml", new Tuple<Type, Type>(typeof(ConnectionProfile), typeof(DataHub)));
                webServer.PagedViewModelTypes.TryAdd("ConnectionProfileTasks.cshtml", new Tuple<Type, Type>(typeof(ConnectionProfileTask), typeof(DataHub)));
                webServer.PagedViewModelTypes.TryAdd("OutputMirrors.cshtml", new Tuple<Type, Type>(typeof(OutputMirror), typeof(DataHub)));
                webServer.PagedViewModelTypes.TryAdd("Settings.cshtml", new Tuple<Type, Type>(typeof(Setting), typeof(DataHub)));
            }
            catch (Exception ex)
            {
                LogException(new InvalidOperationException($"Failed during web-server initialization: {ex.Message}", ex));
                return;
            }

            const int RetryDelay = 1000;
            const int SleepTime = 200;
            const int LoopCount = RetryDelay / SleepTime;

            while (!m_serviceStopping)
            {
                if (TryStartWebHosting(webHostURL))
                {
                    try
                    {
                        // Load target minifier into app domain before precompiling Razor views - this will fix race condition on which assembly to use
                        Minifier _ = new();
                        
                        // Initiate pre-compile of base templates
                        RazorEngine<CSharpEmbeddedResource>.Default.PreCompile(LogException, "GSF.Web.Security.Views.");
                        RazorEngine<CSharpEmbeddedResource>.Default.PreCompile(LogException, "GSF.Web.Shared.Views.");
                        RazorEngine<CSharpEmbeddedResource>.Default.PreCompile(LogException);
                        RazorEngine<CSharp>.Default.PreCompile(LogException);
                    }
                    catch (Exception ex)
                    {
                        LogException(new InvalidOperationException($"Failed to initiate pre-compile of razor templates: {ex.Message}", ex));
                    }

                    break;
                }

                for (int i = 0; i < LoopCount && !m_serviceStopping; i++)
                    Thread.Sleep(SleepTime);
            }
        })
        {
            IsBackground = true
        };
        
        startWebServer.Start();
    }

    private bool TryStartWebHosting(string webHostURL)
    {
        try
        {
            // Create new web application hosting environment
            m_webAppHost = WebApp.Start<Startup>(webHostURL);
            return true;
        }
        catch (TargetInvocationException ex)
        {
            LogException(new InvalidOperationException($"Failed to initialize web hosting: {ex.InnerException?.Message ?? ex.Message}", ex.InnerException ?? ex));
            return false;
        }
        catch (Exception ex)
        {
            LogException(new InvalidOperationException($"Failed to initialize web hosting: {ex.Message}", ex));
            return false;
        }
    }

    /// <summary>Event handler for service stopping operation.</summary>
    /// <param name="sender">Event source.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    /// Time-series framework uses this handler to un-wire events and dispose of system objects.
    /// </remarks>
    protected override void ServiceStoppingHandler(object sender, EventArgs e)
    {
        m_serviceStopping = true;

        ServiceHelper helper = ServiceHelper;

        try
        {
            base.ServiceStoppingHandler(sender, e);
        }
        catch (Exception ex)
        {
            LogException(new InvalidOperationException($"Service stopping handler exception: {ex.Message}", ex));
        }

        if (helper is null)
            return;

        helper.UpdatedStatus -= UpdatedStatusHandler;
        helper.LoggedException -= LoggedExceptionHandler;
    }

    private void WebServer_StatusMessage(object sender, EventArgs<string> e) =>
        LogWebHostStatusMessage(e.Argument);

    public void LogWebHostStatusMessage(string message, UpdateType type = UpdateType.Information) =>
        LogStatusMessage($"[WEBHOST] {message}", type);

    /// <summary>
    /// Logs a status message to connected clients.
    /// </summary>
    /// <param name="message">Message to log.</param>
    /// <param name="type">Type of message to log.</param>
    public void LogStatusMessage(string message, UpdateType type = UpdateType.Information) =>
        DisplayStatusMessage(message, type);

    /// <summary>
    /// Logs an exception to the service.
    /// </summary>
    /// <param name="ex">Exception to log.</param>
    public new void LogException(Exception ex)
    {
        base.LogException(ex);
        DisplayStatusMessage($"{ex.Message}", UpdateType.Alarm);
    }

    /// <summary>
    /// Queues group of tasks or individual task, identified by <paramref name="taskID"/> for execution at specified <paramref name="priority"/>.
    /// When a configured set of pool machines are defined, this method will distribute queue requests across responding machines.
    /// </summary>
    /// <param name="acronyms">Targets <see cref="Downloader"/> device instance acronyms, as defined in database configuration.</param>
    /// <param name="taskID">Task identifier, i.e., the group task identifier or specific task name. Value is not case sensitive.</param>
    /// <param name="priority">Priority of task to use when queuing.</param>
    /// <remarks>
    /// When not providing a specific task name to execute in the <paramref name="taskID"/> parameter,
    /// there are three group-based task identifiers available:
    /// <list type="bullet">
    /// <item><description><c><see cref="Downloader.AllTasksGroupID">_AllTasksGroup_</see></c></description></item>
    /// <item><description><c><see cref="Downloader.ScheduledTasksGroupID">_ScheduledTasksGroup_</see></c></description></item>
    /// <item><description><c><see cref="Downloader.OffScheduleTasksGroupID">_OffScheduleTasksGroup_</see></c></description></item>
    /// </list>
    /// The <c>_AllTasksGroup_</c> task identifier will queue all available tasks for execution, whereas, the
    /// <c>_ScheduledTasksGroup_</c> task identifier will only queue the tasks that share a common primary schedule.
    /// The <c>_OffScheduleTasksGroup_</c> task identifier will queue all tasks that have an overridden schedule
    /// defined. Note that when the <paramref name="taskID"/> is one of the specified group task identifiers, the
    /// queued tasks will execute immediately, regardless of any specified schedule, overridden or otherwise.
    /// </remarks>
    public void QueueTasks(string[] acronyms, string taskID, QueuePriority priority)
    {
        string[] pooledMachines = Model.Global.PoolMachines;

        // When no pooled machines are defined, openMIC instance is either a subordinate in a pool or an
        // independent instance, either way, these cases should handle request to queue tasks "locally":
        if (pooledMachines == null || pooledMachines.Length == 0)
        {
            foreach (string acronym in acronyms)
                QueueTasksLocally(acronym, taskID, priority);
            return;
        }

        // Just using a simple round-robin distribution strategy - each individual system will
        // handle its own priority task execution queue
        // Identify All Machines in the pool that are available to handle the request
        string[] availableMachines = pooledMachines.Where(tm => IsAvailableForQueue(tm) || tm.Equals("localhost")).ToArray();

        HashSet<string> availableAcronyms = acronyms.ToHashSet();

        while (availableAcronyms.Count > 0 )
        {
            if (availableMachines.Length == 0)
            {
                LogStatusMessage($"REMOTE QUEUE: No available pool machines to handle queue request. Queueing tasks locally.", UpdateType.Warning);
                foreach (string acronym in acronyms)
                    QueueTasksLocally(acronym, taskID, priority);
                //This case completly exita since there are no available machines to handle the request, so we will just queue everything locally.
                return;
            }

            // Split the request across the available machines
            
            List<(string targetMachine, string[] targetAcronyms)> requests = availableAcronyms.Select((acronym, index) => (acronym,index)).GroupBy((item) => item.index%availableMachines.Length)
                .Select((grp) => (availableMachines[grp.Key], grp.Select(item => item.acronym).ToArray())).ToList();

            HashSet<string> failedTargetMachines = new();

            int i = 0;

            while(i < requests.Count)
            {
                if (requests[i].targetMachine.Equals("localhost"))
                {
                    foreach (string acronym in acronyms)
                    {
                        QueueTasksLocally(acronym, taskID, priority);
                        availableAcronyms.Remove(acronym);
                    }
                    i++;
                    continue;
                }

                // Queue to each Machine
                string[] targetAcronyms = acronyms; // Avoid modified closure issue in async callback

                string actionURI = $"{GetTargetURI(requests[i].targetMachine)}/api/Operations/QueueTasks?taskID={UrlEncode(taskID)}&priority={UrlEncode(priority.ToString())}";
                HttpRequestMessage request = new(HttpMethod.Post, actionURI);
                request.Content = new StringContent($"[\"{string.Join("\",\"", acronyms)}\"]", System.Text.Encoding.UTF8, "application/json");

                void Succeed(HttpResponseMessage response)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        LogStatusMessage($"REMOTE QUEUE: Successfully executed remote queue action \"{actionURI}\" for \"{acronyms.Length}\" tasks load at \"{priority}\" priority");
                        foreach (string acronym in targetAcronyms)
                            availableAcronyms.Remove(acronym);

                    }
                    else
                        throw new Exception($"REMOTE QUEUE: Failed to execute remote queue action \"{actionURI}\" for \"{acronyms.Length}\" tasks, HTTP response = {response.StatusCode}: {response.ReasonPhrase}");
                }

                void Fail(Exception ex)
                {
                    if (ex is AggregateException aggEx)
                        LogException(new Exception($"REMOTE QUEUE: Failed to execute remote queue action \"{actionURI}\" for \"{acronyms.Length}\": {string.Join(", ", aggEx.InnerExceptions.Select(innerEx => innerEx.Message))}", ex));
                    else
                        LogException(ex);

                    // Requeue On first Failure
                    if (!failedTargetMachines.Contains(requests[i].targetMachine))
                        requests.Add(requests[i]);

                    failedTargetMachines.Add(requests[i].targetMachine);
                }

                try
                {
                    s_http.SendAsync(request).ContinueWith(task =>
                    {
                        try { Succeed(task.Result); }
                        catch (Exception ex) { Fail(ex); }
                    });
                }
                catch (Exception ex)
                {
                    Fail(ex);
                }

                i++;
            }

            // remove any machines that failed on retry from the pool of available machines
            availableMachines = availableMachines.Where(tm => !failedTargetMachines.Contains(tm)).ToArray();
        }
    }

    /// <summary>
    /// Checks if a target instance3 is available to handle a request to queue tasks.
    /// This is done by sending a request to the target instance's web service API and checking for a successful response.
    /// and ensuring the target machine has the same version of OpenMIC.
    /// </summary>
    /// <param name="targetMachine"></param>
    /// <returns></returns>
    private bool IsAvailableForQueue(string targetMachine)
    {
        try
        {
            HttpResponseMessage response = s_http.GetAsync($"{GetTargetURI(targetMachine)}/api/Operations/Version").Result;
            if (response.StatusCode != HttpStatusCode.OK)
                return false;

            return response.Content.ReadAsStringAsync().Result == Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        catch (Exception ex)
        {
            LogException(new InvalidOperationException($"Failed to check availability of pool machine \"{targetMachine}\" for queueing tasks: {ex.Message}", ex));
            return false;
        }
    }

    private string GetTargetURI(string targetMachine)
    {
        if (targetMachine.Contains("://"))
        {
            // Pooled target machine specification includes scheme and possible port number
            return targetMachine;
        }
        else
        {
            // Pooled target machine specification is only target machine name, assume same
            // scheme and port number as local instance
            Uri webHostUri = Model.Global.WebHostUri;
            return $"{webHostUri.Scheme}://{targetMachine}:{webHostUri.Port}";
        }
    }

    private void QueueTasksLocally(string acronym, string taskID, QueuePriority priority)
    {
        IAdapter adapter = GetRequestedAdapter(new ClientRequestInfo(null, ClientRequest.Parse($"invoke {acronym}")));

        if (adapter is Downloader downloader)
            downloader.QueueTasksByID(taskID, priority);
    }

    /// <summary>
    /// Sends a command request to the service.
    /// </summary>
    /// <param name="commandInput">Request string.</param>
    /// <param name="clientID">Client ID of sender.</param>
    /// <param name="principal">The principal used for role-based security.</param>
    public void SendRequest(string commandInput, Guid clientID = default, IPrincipal principal = null)
    {
        if (string.IsNullOrWhiteSpace(commandInput))
            return;

        commandInput = commandInput.Trim();

        ClientRequest request = ClientRequest.Parse(commandInput);

        if (request == null)
            return;

        if (principal != null && SecurityProviderUtility.IsResourceSecurable(request.Command) && !SecurityProviderUtility.IsResourceAccessible(request.Command, principal))
        {
            ServiceHelper.UpdateStatus(clientID, UpdateType.Alarm, $"Access to \"{request.Command}\" is denied.\r\n\r\n");
            return;
        }

        ClientRequestHandler requestHandler = ServiceHelper.FindClientRequestHandler(request.Command);

        if (requestHandler == null)
        {
            ServiceHelper.UpdateStatus(clientID, UpdateType.Alarm, $"Command \"{request.Command}\" is not supported.\r\n\r\n");
            return;
        }

        ClientInfo clientInfo = new() { ClientID = clientID };
        clientInfo.SetClientUser(principal ?? Thread.CurrentPrincipal);

        ClientRequestInfo requestInfo = new(clientInfo, request);
        requestHandler.HandlerMethod(requestInfo);

        GlobalSettings settings = Program.Host.Model.Global;

        // Request task considered complete for remote or standalone instances
        if (settings.UseRemoteScheduler || settings.PoolMachines is null || settings.PoolMachines.Length == 0)
            return;

        // Calls to service for device initializations and configuration reloads should be propagated to pool machines
        if (CommandAllowedForRelay(commandInput))
        {
            Task.Run(async () =>
            {
                try
                {
                    // Return cumulated stats for all pool machines from primary scheduler
                    foreach (string targetMachine in settings.PoolMachines)
                    {
                        if (targetMachine.Equals("localhost"))
                            continue;

                        string targetUri;

                        if (targetMachine.Contains("://"))
                        {
                            // Pooled target machine specification includes scheme and possible port number
                            targetUri = targetMachine;
                        }
                        else
                        {
                            // Pooled target machine specification is only target machine name, assume same
                            // scheme and port number as local instance
                            Uri webHostUri = settings.WebHostUri;
                            targetUri = $"{webHostUri.Scheme}://{targetMachine}:{webHostUri.Port}";
                        }

                        try
                        {
                            string actionURI = $"{targetUri}/api/Operations/RelayCommand?command={UrlEncode(commandInput)}";

                            HttpResponseMessage response = await s_http.SendAsync(new HttpRequestMessage(HttpMethod.Get, actionURI));

                            LogStatusMessage(response.StatusCode == HttpStatusCode.OK ?
                                                 $"Successfully relayed \"{request.Command}\" command to \"{targetMachine}\"." :
                                                 $"Failed to relay \"{request.Command}\" command to \"{targetMachine}\": {response.ReasonPhrase} [{response.StatusCode}:{(int)response.StatusCode}].");
                        }
                        catch (Exception ex)
                        {
                            LogException(new InvalidOperationException($"Failed to relay command \"{request.Command}\" to \"{targetMachine}\": {ex.Message}", ex));
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogException(new InvalidOperationException($"Failed to relay service command \"{commandInput}\": {ex.Message}", ex));
                }
            });
        }
    }

    public void DisconnectClient(Guid clientID)
    {
        ServiceHelper.DisconnectClient(clientID);
    }

    private void UpdatedStatusHandler(object sender, EventArgs<Guid, string, UpdateType> e) =>
        UpdatedStatus?.Invoke(sender, new EventArgs<Guid, string, UpdateType>(e.Argument1, e.Argument2, e.Argument3));

    private void LoggedExceptionHandler(object sender, EventArgs<Exception> e) =>
        LoggedException?.Invoke(sender, new EventArgs<Exception>(e.Argument));

    #endregion

    #region [ Static ]

    // Static Fields
    private static readonly HttpClient s_http = new(new HttpClientHandler { UseCookies = false });

    internal static bool CommandAllowedForRelay(string commandInput) =>
        // Only allow single line commands
        commandInput.Split(new[] { Environment.NewLine, "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries).Length == 1 &&
        (
            // Only allow Initialize and ReloadConfig commands for pool machine relay
            commandInput.StartsWith("init", StringComparison.OrdinalIgnoreCase) ||
            commandInput.StartsWith("reloadconfig", StringComparison.OrdinalIgnoreCase)
        );

    #endregion
}