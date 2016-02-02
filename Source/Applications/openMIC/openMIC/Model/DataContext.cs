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
using GSF;
using GSF.Collections;
using GSF.Data;
using GSF.IO;
using RazorEngine.Templating;

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
        private string m_addInputFieldTemplate;
        private string m_addSelectFieldTemplate;
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

        /// <summary>
        /// Gets the <see cref="AdoDataConnection"/> for this <see cref="DataContext"/>.
        /// </summary>
        public AdoDataConnection Connection => m_connection ?? (m_connection = new AdoDataConnection(m_settingsCategory));

        /// <summary>
        /// Gets the input field razor template file name.
        /// </summary>
        public string AddInputFieldTemplate => m_addInputFieldTemplate ?? (m_addInputFieldTemplate = FilePath.GetAbsolutePath($"{FilePath.AddPathSuffix(Program.Host.WebRootFolder)}AddInputField.cshtml"));

        /// <summary>
        /// Gets the select field razor template file name.
        /// </summary>
        public string AddSelectFieldTemplate => m_addSelectFieldTemplate ?? (m_addSelectFieldTemplate = FilePath.GetAbsolutePath($"{FilePath.AddPathSuffix(Program.Host.WebRootFolder)}AddSelectField.cshtml"));

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
        /// <param name="inputType">Input field type, defaults to text.</param>
        /// <param name="inputLabel">Label name for input text field, defaults to <paramref name="fieldName"/>.</param>
        /// <returns>Generated HTML for new text field based on modeled table field attributes.</returns>
        public string AddInputField<T>(string fieldName, string inputType = null, string inputLabel = null) where T : class, new()
        {
            RazorView<CSharp> addInputFieldTemplate = new RazorView<CSharp>(AddInputFieldTemplate, Program.Host.Model);
            DynamicViewBag viewBag = addInputFieldTemplate.ViewBag;
            TableOperations<T> tableOperations = Table<T>();
            RequiredAttribute requiredAttribute;
            StringLengthAttribute stringLengthAttribute;

            tableOperations.TryGetFieldAttribute(fieldName, out requiredAttribute);
            tableOperations.TryGetFieldAttribute(fieldName, out stringLengthAttribute);

            viewBag.AddValue("Required", (object)requiredAttribute != null);
            viewBag.AddValue("StringLength", stringLengthAttribute?.MaximumLength ?? 0);
            viewBag.AddValue("FieldName", fieldName);
            viewBag.AddValue("InputType", inputType ?? "text");
            viewBag.AddValue("InputLabel", inputLabel ?? fieldName);

            return addInputFieldTemplate.Execute();
        }

        /// <summary>
        /// Generates select field based on reflected modeled table field attributes.
        /// </summary>
        /// <typeparam name="TSelect">Modeled table for select field.</typeparam>
        /// <typeparam name="TOption">Modeled table for option data.</typeparam>
        /// <param name="fieldName">Field name for value of select field.</param>
        /// <param name="optionFieldID">Field name for ID of option data.</param>
        /// <param name="optionFieldLabel">Field name for label of option data, defaults to <paramref name="optionFieldID"/></param>
        /// <param name="selectLabel">Label name for select field, defaults to <paramref name="fieldName"/>.</param>
        /// <returns>Generated HTML for new text field based on modeled table field attributes.</returns>
        public string AddSelectField<TSelect, TOption>(string fieldName, string optionFieldID, string optionFieldLabel = null, string selectLabel = null) where TSelect : class, new() where TOption : class, new()
        {
            RazorView<CSharp> addSelectFieldTemplate = new RazorView<CSharp>(AddSelectFieldTemplate, Program.Host.Model);
            DynamicViewBag viewBag = addSelectFieldTemplate.ViewBag;
            TableOperations<TSelect> selectTableOperations = Table<TSelect>();
            TableOperations<TOption> optionTableOperations = Table<TOption>();
            RequiredAttribute requiredAttribute;
            Dictionary<string, string> options = new Dictionary<string, string>();
            string tableName = typeof(TOption).Name;

            selectTableOperations.TryGetFieldAttribute(fieldName, out requiredAttribute);

            optionFieldLabel = optionFieldLabel ?? optionFieldID;
            selectLabel = selectLabel ?? tableName;

            viewBag.AddValue("Required", (object)requiredAttribute != null);
            viewBag.AddValue("FieldName", fieldName);
            viewBag.AddValue("SelectLabel", selectLabel);

            foreach (TOption record in QueryRecords<TOption>($"SELECT {optionFieldID} FROM {tableName} ORDER BY {optionFieldLabel}"))
                options.Add(optionTableOperations.GetFieldValue(record, optionFieldID).ToString(), optionTableOperations.GetFieldValue(record, optionFieldLabel).ToNonNullString(selectLabel));

            viewBag.AddValue("Options", options);

            return addSelectFieldTemplate.Execute();
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
