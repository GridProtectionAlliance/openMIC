//******************************************************************************************************
//  RazorView.cs - Gbtc
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
//  01/13/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Web;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;

// ReSharper disable StaticMemberInGenericType
namespace openMIC
{
    public abstract class LanguageContraint
    {
        public abstract Language TargetLanguage
        {
            get;
        }
    }

    public class CSharp : LanguageContraint
    {
        public override Language TargetLanguage => Language.CSharp;
    }

    public class VisualBasic : LanguageContraint
    {
        public override Language TargetLanguage => Language.VisualBasic;
    }

    public class RazorView<T> where T : LanguageContraint, new()
    {
        #region [ Members ]

        // Fields
        private readonly DynamicViewBag m_viewBag = new DynamicViewBag();

        #endregion

        #region [ Constructors ]

        public RazorView()
        {
        }

        public RazorView(string templateName)
        {
            this.TemplateName = templateName;
        }

        public RazorView(string templateName, object model) : this(templateName)
        {
            this.Model = model;
        }

        #endregion

        #region [ Properties ]

        public string TemplateName
        {
            get; set;
        }

        public object Model
        {
            get; set;
        }

        public dynamic ViewBag => m_viewBag;

        #endregion

        #region [ Methods ]

        public string Run()
        {
            return s_engineService.RunCompile(TemplateName, null, Model, m_viewBag);
        }

        #endregion

        #region [ Static ]

        // Static Fields
        private static readonly IRazorEngineService s_engineService;

        // Static Constructor
        static RazorView()
        {
            T languageType = new T();

#if DEBUG
            // The watching resolve path template manager should not be used in production since
            // assemblies cannot be unloaded from an AppDomain. Every time a change to a .cshtml
            // file is picked up by the watcher it is compiled and loaded into the AppDomain and
            // the old one cannot be removed (.NET restriction), the net result is a memory leak
            InvalidatingCachingProvider cachingProvider = new InvalidatingCachingProvider(fileName => Program.Host.LogStatusMessage($"Cache invalidated for {fileName}..."));

            s_engineService = RazorEngineService.Create(new TemplateServiceConfiguration
            {
                Language = languageType.TargetLanguage,
                CachingProvider = cachingProvider,
                TemplateManager = new WatchingResolvePathTemplateManager(new[] { Program.Host.WebRootFolder }, cachingProvider),
                Debug = true
            });
#else
            s_engineService = RazorEngineService.Create(new TemplateServiceConfiguration
            {
                Language  = languageType.TargetLanguage,
                TemplateManager = new ResolvePathTemplateManager(new[] { Program.Host.WebRootFolder }),
                Debug = false
            });
#endif
        }

        #endregion
    }
}
