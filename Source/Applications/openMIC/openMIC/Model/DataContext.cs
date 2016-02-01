//******************************************************************************************************
//  DataContext.cs - Gbtc
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
//  02/01/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using GSF.Collections;
using GSF.Data;

namespace openMIC.Model
{
    /// <summary>
    /// Defines a data context for the current model.
    /// </summary>
    public class DataContext : IDisposable
    {
        #region [ Members ]

        // Fields
        private AdoDataConnection m_connection;
        private readonly Dictionary<Type, object> m_tableOperations;
        private readonly string m_settingsCategory;
        private readonly bool m_disposeConnection;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="DataContext"/>.
        /// </summary>
        /// <param name="connection"><see cref="AdoDataConnection"/> to use; defaults to a new connection.</param>
        /// <param name="disposeConnection">Set to <c>true</c> to dispose the provided <paramref name="connection"/>.</param>
        public DataContext(AdoDataConnection connection = null, bool disposeConnection = false)
        {
            m_connection = connection;
            m_tableOperations = new Dictionary<Type, object>();
            m_settingsCategory = "systemSettings";
            m_disposeConnection = disposeConnection || connection == null;
        }

        /// <summary>
        /// Creates a new <see cref="DataContext"/> using the specified <paramref name="settingsCategory"/>.
        /// </summary>
        /// <param name="settingsCategory">Setting category that contains the connection settings.</param>
        public DataContext(string settingsCategory)
        {
            m_tableOperations = new Dictionary<Type, object>();
            m_settingsCategory = settingsCategory;
            m_disposeConnection = true;
        }

        #endregion

        #region [ Properties ]

        // Only create object instances on demand

        /// <summary>
        /// Gets the <see cref="AdoDataConnection"/> for this <see cref="DataContext"/>.
        /// </summary>
        public AdoDataConnection Connection => m_connection ?? (m_connection = new AdoDataConnection(m_settingsCategory));


        /// <summary>
        /// Gets the table operations for the specified modeled table <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Modeled table.</typeparam>
        /// <returns>Table operations for the specified modeled table <typeparamref name="T"/>.</returns>
        public TableOperations<T> Table<T>() where T : class, new()
        {
            return m_tableOperations.GetOrAdd(typeof(T), (type) => new TableOperations<T>(Connection)) as TableOperations<T>;
        }

        /// <summary>
        /// Queries database and returns modeled table records for the specified sql statement and parameters.
        /// </summary>
        /// <param name="sqlFormat">SQL expression to query.</param>
        /// <param name="parameters">Parameters for query, if any.</param>
        /// <returns>An enumerable of modeled table row instances for queried records.</returns>
        public IEnumerable<T> QueryRecords<T>(string sqlFormat, params object[] parameters) where T : class, new()
        {
            TableOperations<T> tableOperations = Table<T>();

            foreach (DataRow row in m_connection.RetrieveData(sqlFormat, parameters).Rows)
                yield return tableOperations.LoadRecord(tableOperations.GetPrimaryKeys(row));
        }

        /// <summary>
        /// Generates input text field based on reflected modeled table field attributes.
        /// </summary>
        /// <typeparam name="T">Modeled table.</typeparam>
        /// <param name="fieldName">Field name for input text field.</param>
        /// <param name="type">Input field type, defaults to text.</param>
        /// <param name="inputLabel">Label name for input text field, defaults to <paramref name="fieldName"/>.</param>
        /// <returns>Generated HTML for new text field based on modeled table field attributes.</returns>
        public string AddInputField<T>(string fieldName, string type = null, string inputLabel = null) where T : class, new()
        {
            TableOperations<T> tableOperations = Table<T>();
            RequiredAttribute requiredAttribute;
            StringLengthAttribute stringLengthAttribute;

            tableOperations.TryGetFieldAttribute(fieldName, out requiredAttribute);
            tableOperations.TryGetFieldAttribute(fieldName, out stringLengthAttribute);

            bool required = ((object)requiredAttribute != null);
            int stringLength = stringLengthAttribute?.MaximumLength ?? 0;
            type = type ?? "text";
            inputLabel = inputLabel ?? fieldName;

            if (required)
                return string.Format(@"
                    <div class=""form-group"" data-bind=""css: {{'has-error': isEmpty({0}()), 'has-feedback': isEmpty({0}())}}"">
                        <label for=""{0}"">{2}:</label>
                        <input type=""{1}"" class=""form-control"" data-bind=""textInput: {0}"" id=""{0}""{3} required>
                        <span class=""glyphicon glyphicon-remove form-control-feedback"" data-bind=""visible: isEmpty({0}())""></span>
                    </div>
                    ", fieldName, type, inputLabel, stringLength > 0 ? $" maxlength=\"{stringLength}\" size=\"{stringLength}\"" : "");

            return string.Format(@"
                <div class=""form-group"">
                    <label for=""{0}"">{2}</label>
                    <input type=""{1}"" class=""form-control"" data-bind=""textInput: {0}"" id=""{0}""{3}>
                </div>
                ", fieldName, type, inputLabel, stringLength > 0 ? $" maxlength=\"{stringLength}\" size=\"{stringLength}\"" : "");
        }

        /// <summary>
        /// Generates select field based on reflected modeled table field attributes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T">Modeled table.</typeparam>
        /// <param name="fieldName">Field name for value of select field.</param>
        /// <param name="fieldLabel">Field name for label of select field, defaults to <paramref name="fieldName"/></param>
        /// <param name="selectLabel">Label name for select field, defaults to <paramref name="fieldName"/>.</param>
        /// <returns>Generated HTML for new text field based on modeled table field attributes.</returns>
        public string AddSelectField<T>(string fieldName, string fieldLabel = null, string selectLabel = null) where T : class, new()
        {
            StringBuilder options = new StringBuilder();
            TableOperations<T> tableOperations = Table<T>();
            RequiredAttribute requiredAttribute;

            tableOperations.TryGetFieldAttribute(fieldName, out requiredAttribute);

            bool required = ((object)requiredAttribute != null);
            fieldLabel = fieldLabel ?? fieldName;
            selectLabel = selectLabel ?? fieldName;

            foreach (T record in QueryRecords<T>($"SELECT {fieldName}, {fieldLabel} FROM {typeof(T).Name} ORDER BY {fieldLabel}"))
                options.AppendLine($"<option value=\"{tableOperations.GetFieldValue(record, fieldName)}\">{tableOperations.GetFieldValue(record, fieldLabel)}</option>");

            return string.Format(@"
                    <div class=""form-group"">
                        <label for=""{0}"">{1}:</label>
                        <select class=""form-control"" id=""{0}"" data-bind=""value: {0}, optionsCaption: 'Select {1}...', valueAllowUnset: {2}"">
                            {3}
                        </select>
                    </div>
                    ", fieldName, selectLabel, required, options);
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases all the resources used by the <see cref="DataContext"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="DataContext"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    if (disposing)
                    {
                        if (m_disposeConnection)
                            m_connection?.Dispose();
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        #endregion
    }
}
