//******************************************************************************************************
//  APIAuthentication.cs - Gbtc
//
//  Copyright © 2026, Grid Protection Alliance.  All Rights Reserved.
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
//  02/23/2026 - C. Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

using Microsoft.Owin;
using Owin;

namespace openMIC.Authentication
{
    /// <inheritdoc/>
    public partial class APIAuthenticationMiddleware : openXDA.APIMiddleware.APIAuthenticationMiddleware
    {
        #region [ Properties ]

        private string m_apiKey;
        private string m_apiToken;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="APIAuthenticationMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="apiKey">The API key for authentication.</param>
        /// <param name="apiToken">The token required for authorization to web API endpoints.</param>
        public APIAuthenticationMiddleware(OwinMiddleware next, string apiKey, string apiToken)
            : base(next, () => null)
        {
            m_apiKey = apiKey;
            m_apiToken = apiToken;
        }

        #endregion

        #region [ Methods ]

        /// <inheritdoc/>
        protected override string GetAPIToken(string registrationKey)
        {
            if (string.IsNullOrEmpty(registrationKey) || string.IsNullOrEmpty(m_apiKey))
                return null;

            if (registrationKey == m_apiKey)
                return m_apiToken;

            return null;
        }

        /// <inheritdoc/>
        /// <remarks> This implementation does not support impersonation. </remarks>
        protected override bool UseImpersonation(AuthorizationHeader header)
        {
            return false;
        }

        #endregion
    }

    namespace Extensions
    {
        /// <summary>
        /// Extension methods for hosting API authentication.
        /// </summary>
        public static class APIAuthenticationMiddlewareExtensions
        {
            /// <summary>
            /// Enables use of the API authentication middleware in the app.
            /// </summary>
            /// <param name="app">The app in which the middleware will be used.</param>
            /// <param name="apiKey">The key used for API access.</param>
            /// <param name="apiToken">The token used for API access.</param>
            public static void UseAPIAuthentication(this IAppBuilder app, string apiKey, string apiToken) =>
                app.Use<APIAuthenticationMiddleware>(apiKey, apiToken);
        }
    }
}