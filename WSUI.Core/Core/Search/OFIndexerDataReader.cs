﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using OF.Core.Interfaces;

namespace OF.Core.Core.Search
{
    public class OFIndexerDataReader : IIndexerDataReader
    {
        #region [needs]

        private const string ConnectionString = "Provider=Search.CollatorDSO;Extended Properties=\"Application=Windows\"";

        private volatile object _lock = new object();
        private OleDbConnection _connection = null;

        #endregion [needs]

        #region [ctor]

        private OFIndexerDataReader()
        {
            _connection = new OleDbConnection(ConnectionString);
        }

        #endregion [ctor]

        #region [static]

        private static Lazy<IIndexerDataReader> _instance = new Lazy<IIndexerDataReader>(() => new OFIndexerDataReader());

        public static IIndexerDataReader Instance
        {
            get { return _instance.Value; }
        }

        #endregion [static]

        public DataTable GetDataByAdapter(string query)
        {
            DataTable data = null;
            lock (_lock)
            {
                try
                {
                    data = GetDataTableByAdapter(query, _connection);
                }
                finally
                {
                    _connection.Close();
                }
            }
            return data;
        }

        public DataTable GetDataByReader(string query, IEnumerable<string> ingnoredFields)
        {
            DataTable data = null;
            IDataReader reader = null;
            lock (_lock)
                try
                {
                    _connection.Open();
                    OleDbCommand cmd = new OleDbCommand(query, _connection);
                    reader = cmd.ExecuteReader();
                    data = GetDataTableFast(reader, ingnoredFields);
                }
                catch (Exception ex)
                {
                    OF.Core.Logger.OFLogger.Instance.LogError("GetDataByReader: {0}", ex.ToString());
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                    _connection.Close();
                }
            return data;
        }

        public IIndexerDataReader GetInstance()
        {
            return new OFIndexerDataReader();
        }

        #region [private]

        private DataTable GetDataTableFromDataReader(IDataReader reader, IEnumerable<string> ignoredFields)
        {
            DataTable table = reader.GetSchemaTable();
            DataTable resultTable = new DataTable();
            List<int> listIndex = new List<int>();
            int index = 0;
            foreach (DataRow dataRow in table.Rows)
            {
                DataColumn dataColumn = new DataColumn();
                dataColumn.ColumnName = dataRow["ColumnName"].ToString();
                dataColumn.DataType = Type.GetType(dataRow["DataType"].ToString());
                dataColumn.ReadOnly = (bool)dataRow["IsReadOnly"];
                dataColumn.AutoIncrement = (bool)dataRow["IsAutoIncrement"];
                dataColumn.Unique = (bool)dataRow["IsUnique"];
                if (ignoredFields != null && ignoredFields.Any(s => s.Equals(dataColumn.ColumnName)))
                    listIndex.Add(index);
                resultTable.Columns.Add(dataColumn);
                index++;
            }

            int row = 0;
            try
            {
                while (reader.Read())
                {
                    row++;
                    DataRow dataRow = resultTable.NewRow();

                    for (int i = 0; i < resultTable.Columns.Count - 1; i++)
                    {
                        if (listIndex.Contains(i))
                            continue;
                        try
                        {
                            object val = reader[i];
                            dataRow[i] = val;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.ToString());
                            continue;
                        }
                    }
                    resultTable.Rows.Add(dataRow);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return resultTable;
        }

        private DataTable GetDataTableByAdapter(string query, OleDbConnection connection)
        {
            DataTable dt = new DataTable();
            new OleDbDataAdapter(query, connection).Fill(dt);
            return dt;
        }

        private DataTable GetDataTableFast(IDataReader rdr, IEnumerable<string> ignoredFields)
        {
            DataTable resultTable = GetDataTableFromDataReader(rdr, ignoredFields);
            return resultTable;
        }

        #endregion [private]
    }
}