//******************************************************************************************************
//  Sql.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  01/30/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using GSF.Data;
using GSF.Reflection;

namespace openMIC.Model
{
    public class Sql<T> where T : class, new()
    {
        private const string CountSqlFormat = "SELECT COUNT(*) FROM {0}";
        private const string OrderBySqlFormat = "SELECT {0} FROM {1} ORDER BY {{0}}";
        private const string SelectSqlFormat = "SELECT * FROM {0} WHERE {1}";
        private const string AddNewSqlFormat = "INSERT INTO {0}({1}) VALUES ({2})";
        private const string UpdateSqlFormat = "UPDATE {0} SET {1} WHERE {2}";
        private const string DeleteSqlFormat = "DELETE FROM {0} WHERE {1}";

        public static readonly PropertyInfo[] Properties;
        public static readonly PropertyInfo[] AddNewProperties;
        public static readonly PropertyInfo[] UpdateProperties;
        public static readonly PropertyInfo[] PrimaryKeyProperties;

        public static readonly string TableName;
        public static readonly string CountSql;
        public static readonly string OrderBySql;
        public static readonly string SelectSql;
        public static readonly string AddNewSql;
        public static readonly string UpdateSql;
        public static readonly string DeleteSql;

        private IEnumerable<DataRow> m_primaryKeyCache;
        private string m_lastSortField;

        public IEnumerable<T> QueryRecords(AdoDataConnection connection, string sortField, bool ascending, int page, int pageSize)
        {
            if ((object)m_primaryKeyCache == null || string.Compare(sortField, m_lastSortField, true) != 0)
            {
                m_primaryKeyCache = connection.RetrieveData(string.Format(OrderBySql, $"{sortField}{(ascending ? "" : " DESC")}")).AsEnumerable();
                m_lastSortField = sortField;
            }

            foreach (DataRow row in m_primaryKeyCache.ToPagedList(page, pageSize))
                yield return LoadRecord(connection, row.ItemArray);
        }

        public static int QueryRecordCount(AdoDataConnection connection)
        {
            return connection.ExecuteScalar<int>(CountSql);
        }

        public static T LoadRecord(AdoDataConnection connection, params object[] primaryKeys)
        {
            T record = new T();
            DataRow row = connection.RetrieveRow(string.Format(SelectSql, primaryKeys));

            foreach (PropertyInfo property in Properties)
                property.SetValue(record, row[property.Name], null);

            return record;
        }

        public static void DeleteRecord(AdoDataConnection connection, params object[] primaryKeys)
        {
            connection.ExecuteNonQuery(DeleteSql, primaryKeys);
        }

        public static void UpdateRecord(AdoDataConnection connection, T record)
        {
            List<object> values = new List<object>();

            foreach (PropertyInfo property in UpdateProperties)
                values.Add(property.GetValue(record));

            foreach (PropertyInfo property in PrimaryKeyProperties)
                values.Add(property.GetValue(record));

            connection.ExecuteNonQuery(UpdateSql, values.ToArray());
        }

        public static void AddNewRecord(AdoDataConnection connection, T record)
        {
            List<object> values = new List<object>();

            foreach (PropertyInfo property in AddNewProperties)
                values.Add(property.GetValue(record));

            connection.ExecuteNonQuery(AddNewSql, values.ToArray());
        }

        static Sql()
        {
            StringBuilder addNewFields = new StringBuilder();
            StringBuilder addNewFormat = new StringBuilder();
            StringBuilder updateFormat = new StringBuilder();
            StringBuilder whereFormat = new StringBuilder();
            StringBuilder primaryKeyFields = new StringBuilder();
            List<PropertyInfo> addNewProperties = new List<PropertyInfo>();
            List<PropertyInfo> updateproperties = new List<PropertyInfo>();
            List<PropertyInfo> primaryKeyProperties = new List<PropertyInfo>();
            PrimaryKeyAttribute primaryKeyAttribute;
            string tableName = typeof(T).Name;
            int primaryKeyIndex = 0;
            int addNewFieldIndex = 0;
            int updateFieldIndex = 0;
 
            Properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.CanRead && property.CanWrite).ToArray();

            foreach (PropertyInfo property in Properties)
            {
                property.TryGetAttribute(out primaryKeyAttribute);

                if ((object)primaryKeyAttribute != null)
                {
                    if (!primaryKeyAttribute.IsIdentity)
                    {
                        addNewFields.Append($"{(addNewFields.Length > 0 ? ", " : "")}{property.Name}");
                        addNewFormat.Append($"{(addNewFormat.Length > 0 ? ", " : "")}{{{addNewFieldIndex++}}}");
                        addNewProperties.Add(property);
                    }

                    whereFormat.Append($"{(whereFormat.Length > 0 ? "AND " : "")}{property.Name}={{{primaryKeyIndex++}}}");
                    primaryKeyFields.Append($"{(primaryKeyFields.Length > 0 ? ", " : "")}{property.Name}");
                    primaryKeyProperties.Add(property);
                }
                else
                {
                    addNewFields.Append($"{(addNewFields.Length > 0 ? ", " : "")}{property.Name}");
                    addNewFormat.Append($"{(addNewFormat.Length > 0 ? ", " : "")}{{{addNewFieldIndex++}}}");
                    updateFormat.Append($"{(updateFormat.Length > 0 ? ", " : "")}{property.Name}={{{updateFieldIndex++}}}");
                    addNewProperties.Add(property);
                    updateproperties.Add(property);
                }
            }

            List<object> updateWhereOffsets = new List<object>();

            for (int i = 0; i < primaryKeyIndex; i++)
                updateWhereOffsets.Add($"{{{updateFieldIndex + i}}}");

            CountSql = string.Format(CountSqlFormat, tableName);
            OrderBySql = string.Format(OrderBySqlFormat, primaryKeyFields, tableName);
            SelectSql = string.Format(SelectSqlFormat, tableName, whereFormat);
            AddNewSql = string.Format(AddNewSqlFormat, tableName, addNewFields, addNewFormat);
            UpdateSql = string.Format(UpdateSqlFormat, tableName, updateFormat, string.Format(whereFormat.ToString(), updateWhereOffsets.ToArray()));
            DeleteSql = string.Format(DeleteSqlFormat, tableName, whereFormat);

            AddNewProperties = addNewProperties.ToArray();
            UpdateProperties = updateproperties.ToArray();
            PrimaryKeyProperties = primaryKeyProperties.ToArray();
        }
    }
}
