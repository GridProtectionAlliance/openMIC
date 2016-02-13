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
using System.Linq;
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
        private readonly Dictionary<string, Tuple<string, string>> m_fieldValidationParameters;
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
            m_fieldValidationParameters = new Dictionary<string, Tuple<string, string>>();
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
            m_fieldValidationParameters = new Dictionary<string, Tuple<string, string>>();
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
        /// Gets validation pattern and error message for rendered fields, if any.
        /// </summary>
        public Dictionary<string, Tuple<string, string>> FieldValidationParameters => m_fieldValidationParameters;

        /// <summary>
        /// Gets the table operations for the specified modeled table <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Modeled table.</typeparam>
        /// <returns>Table operations for the specified modeled table <typeparamref name="T"/>.</returns>
        public TableOperations<T> Table<T>() where T : class, new()
        {
            return m_tableOperations.GetOrAdd(typeof(T), type => new TableOperations<T>(Connection)) as TableOperations<T>;
        }

        /// <summary>
        /// Queries database and returns modeled table records for the specified sql statement and parameters.
        /// </summary>
        /// <param name="sqlFormat">SQL expression to query.</param>
        /// <param name="parameters">Parameters for query, if any.</param>
        /// <returns>An enumerable of modeled table row instances for queried records.</returns>
        public IEnumerable<T> QueryRecords<T>(string sqlFormat, params object[] parameters) where T : class, new()
        {
            try
            {
                TableOperations<T> tableOperations = Table<T>();
                return m_connection.RetrieveData(sqlFormat, parameters).AsEnumerable().Select(row => tableOperations.LoadRecord(tableOperations.GetPrimaryKeys(row)));
            }
            catch (Exception ex)
            {
                Program.Host.LogException(new InvalidOperationException($"Exception during record query for {typeof(T).Name} \"{sqlFormat}{ParamList(parameters)}\": {ex.Message}", ex));
                return Enumerable.Empty<T>();
            }
        }

        /// <summary>
        /// Adds a new pattern based validation and option error message to a field.
        /// </summary>
        /// <param name="observableFieldReference">Observable field reference (from JS view model).</param>
        /// <param name="validationPattern">Regex based validation pattern.</param>
        /// <param name="errorMessage">Optional error message to display when pattern fails.</param>
        public void AddFieldValidation(string observableFieldReference, string validationPattern, string errorMessage = null)
        {
            m_fieldValidationParameters[observableFieldReference] = new Tuple<string, string>(validationPattern, errorMessage);
        }

        /// <summary>
        /// Generates template based input text field based on reflected modeled table field attributes.
        /// </summary>
        /// <typeparam name="T">Modeled table.</typeparam>
        /// <param name="fieldName">Field name for input text field.</param>
        /// <param name="inputType">Input field type, defaults to text.</param>
        /// <param name="inputLabel">Label name for input text field, defaults to <paramref name="fieldName"/>.</param>
        /// <param name="fieldID">ID to use for input field; defaults to input + <paramref name="fieldName"/>.</param>
        /// <param name="groupDataBinding">Data-bind operations to apply to outer form-group div, if any.</param>
        /// <param name="labelDataBinding">Data-bind operations to apply to label, if any.</param>
        /// <param name="customDataBinding">Extra custom data-binding operations to apply to field, if any.</param>
        /// <param name="toolTip">Tool tip text to apply to field, if any.</param>
        /// <returns>Generated HTML for new text field based on modeled table field attributes.</returns>
        public string AddInputField<T>(string fieldName, string inputType = null, string inputLabel = null, string fieldID = null, string groupDataBinding = null, string labelDataBinding = null, string customDataBinding = null, string toolTip = null) where T : class, new()
        {
            TableOperations<T> tableOperations = Table<T>();
            StringLengthAttribute stringLengthAttribute;
            RegularExpressionAttribute regularExpressionAttribute;

            tableOperations.TryGetFieldAttribute(fieldName, out stringLengthAttribute);
            tableOperations.TryGetFieldAttribute(fieldName, out regularExpressionAttribute);

            if (!string.IsNullOrEmpty(regularExpressionAttribute?.ErrorMessage))
            {
                string observableReference;

                if (string.IsNullOrEmpty(groupDataBinding))
                    observableReference = $"viewModel.currentRecord().{fieldName}";
                else // "with: $root.connectionString"
                    observableReference = $"viewModel.{groupDataBinding.Substring(groupDataBinding.IndexOf('.') + 1)}";

                AddFieldValidation(observableReference, regularExpressionAttribute.Pattern, regularExpressionAttribute.ErrorMessage);
            }

            return AddInputField(fieldName, tableOperations.FieldHasAttribute<RequiredAttribute>(fieldName),
                stringLengthAttribute?.MaximumLength ?? 0, inputType, inputLabel, fieldID, groupDataBinding, labelDataBinding, customDataBinding, toolTip);
        }

        /// <summary>
        /// Generates template based input text field based on specified parameters.
        /// </summary>
        /// <param name="fieldName">Field name for input text field.</param>
        /// <param name="required">Determines if field name is required.</param>
        /// <param name="maxLength">Defines maximum input field length.</param>
        /// <param name="inputType">Input field type, defaults to text.</param>
        /// <param name="inputLabel">Label name for input text field, defaults to <paramref name="fieldName"/>.</param>
        /// <param name="fieldID">ID to use for input field; defaults to input + <paramref name="fieldName"/>.</param>
        /// <param name="groupDataBinding">Data-bind operations to apply to outer form-group div, if any.</param>
        /// <param name="labelDataBinding">Data-bind operations to apply to label, if any.</param>
        /// <param name="customDataBinding">Extra custom data-binding operations to apply to field, if any.</param>
        /// <param name="toolTip">Tool tip text to apply to field, if any.</param>
        /// <returns>Generated HTML for new text field based on modeled table field attributes.</returns>
        public string AddInputField(string fieldName, bool required, int maxLength = 0, string inputType = null, string inputLabel = null, string fieldID = null, string groupDataBinding = null, string labelDataBinding = null, string customDataBinding = null, string toolTip = null)
        {
            RazorView<CSharp> addInputFieldTemplate = new RazorView<CSharp>(AddInputFieldTemplate, Program.Host.Model);
            DynamicViewBag viewBag = addInputFieldTemplate.ViewBag;

            viewBag.AddValue("FieldName", fieldName);
            viewBag.AddValue("Required", required);
            viewBag.AddValue("MaxLength", maxLength);
            viewBag.AddValue("InputType", inputType ?? "text");
            viewBag.AddValue("InputLabel", inputLabel ?? fieldName);
            viewBag.AddValue("FieldID", fieldID ?? $"input{fieldName}");
            viewBag.AddValue("GroupDataBinding", groupDataBinding);
            viewBag.AddValue("LabelDataBinding", labelDataBinding);
            viewBag.AddValue("CustomDataBinding", customDataBinding);
            viewBag.AddValue("ToolTip", toolTip);

            return addInputFieldTemplate.Execute();
        }

        /// <summary>
        /// Generates template based select field based on reflected modeled table field attributes.
        /// </summary>
        /// <typeparam name="TSelect">Modeled table for select field.</typeparam>
        /// <typeparam name="TOption">Modeled table for option data.</typeparam>
        /// <param name="fieldName">Field name for value of select field.</param>
        /// <param name="optionValueFieldName">Field name for ID of option data.</param>
        /// <param name="optionLabelFieldName">Field name for label of option data, defaults to <paramref name="optionValueFieldName"/></param>
        /// <param name="selectLabel">Label name for select field, defaults to <paramref name="fieldName"/>.</param>
        /// <param name="fieldID">ID to use for select field; defaults to select + <paramref name="fieldName"/>.</param>
        /// <param name="groupDataBinding">Data-bind operations to apply to outer form-group div, if any.</param>
        /// <param name="labelDataBinding">Data-bind operations to apply to label, if any.</param>
        /// <param name="customDataBinding">Extra custom data-binding operations to apply to field, if any.</param>
        /// <param name="toolTip">Tool tip text to apply to field, if any.</param>
        /// <returns>Generated HTML for new text field based on modeled table field attributes.</returns>
        public string AddSelectField<TSelect, TOption>(string fieldName, string optionValueFieldName, string optionLabelFieldName = null, string selectLabel = null, string fieldID = null, string groupDataBinding = null, string labelDataBinding = null, string customDataBinding = null, string toolTip = null) where TSelect : class, new() where TOption : class, new()
        {
            return AddSelectField<TOption>(fieldName, Table<TSelect>().FieldHasAttribute<RequiredAttribute>(fieldName),
                optionValueFieldName, optionLabelFieldName, selectLabel, fieldID, groupDataBinding, labelDataBinding, customDataBinding, toolTip);
        }

        /// <summary>
        /// Generates template based select field based on reflected modeled table field attributes.
        /// </summary>
        /// <typeparam name="TOption">Modeled table for option data.</typeparam>
        /// <param name="fieldName">Field name for value of select field.</param>
        /// <param name="required">Determines if field name is required.</param>
        /// <param name="optionValueFieldName">Field name for ID of option data.</param>
        /// <param name="optionLabelFieldName">Field name for label of option data, defaults to <paramref name="optionValueFieldName"/></param>
        /// <param name="selectLabel">Label name for select field, defaults to <paramref name="fieldName"/>.</param>
        /// <param name="fieldID">ID to use for select field; defaults to select + <paramref name="fieldName"/>.</param>
        /// <param name="groupDataBinding">Data-bind operations to apply to outer form-group div, if any.</param>
        /// <param name="labelDataBinding">Data-bind operations to apply to label, if any.</param>
        /// <param name="customDataBinding">Extra custom data-binding operations to apply to field, if any.</param>
        /// <param name="toolTip">Tool tip text to apply to field, if any.</param>
        /// <returns>Generated HTML for new text field based on modeled table field attributes.</returns>
        public string AddSelectField<TOption>(string fieldName, bool required, string optionValueFieldName, string optionLabelFieldName = null, string selectLabel = null, string fieldID = null, string groupDataBinding = null, string labelDataBinding = null, string customDataBinding = null, string toolTip = null) where TOption : class, new()
        {
            RazorView<CSharp> addSelectFieldTemplate = new RazorView<CSharp>(AddSelectFieldTemplate, Program.Host.Model);
            DynamicViewBag viewBag = addSelectFieldTemplate.ViewBag;
            TableOperations<TOption> optionTableOperations = Table<TOption>();
            Dictionary<string, string> options = new Dictionary<string, string>();
            string optionTableName = typeof(TOption).Name;

            optionLabelFieldName = optionLabelFieldName ?? optionValueFieldName;
            selectLabel = selectLabel ?? optionTableName;

            viewBag.AddValue("FieldName", fieldName);
            viewBag.AddValue("Required", required);
            viewBag.AddValue("SelectLabel", selectLabel);
            viewBag.AddValue("FieldID", fieldID ?? $"select{fieldName}");
            viewBag.AddValue("GroupDataBinding", groupDataBinding);
            viewBag.AddValue("LabelDataBinding", labelDataBinding);
            viewBag.AddValue("CustomDataBinding", customDataBinding);
            viewBag.AddValue("ToolTip", toolTip);

            foreach (TOption record in QueryRecords<TOption>($"SELECT {optionValueFieldName} FROM {optionTableName} ORDER BY {optionLabelFieldName}"))
                options.Add(optionTableOperations.GetFieldValue(record, optionValueFieldName).ToString(), optionTableOperations.GetFieldValue(record, optionLabelFieldName).ToNonNullString(selectLabel));

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
        private static string ParamList(IReadOnlyList<object> parameters)
        {
            StringBuilder delimitedString = new StringBuilder();


            for (int i = 0; i < parameters.Count; i++)
                delimitedString.AppendFormat(", {0}:{1}", i, parameters[i]);

            return delimitedString.ToString();
        }

        #endregion
    }
}
