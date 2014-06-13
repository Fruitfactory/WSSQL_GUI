using System;
using System.Data;
using System.Data.OleDb;
using WSUI.Core.Interfaces;

namespace WSUI.Core.Core.Search
{
    public class IndexerDataReader : IIndexerDataReader
    {
        #region [needs]

        private const string ConnectionString = "Provider=Search.CollatorDSO;Extended Properties=\"Application=Windows\"";

        private volatile object _lock = new object();
        private OleDbConnection _connection = null;


        #endregion

        #region [ctor]

        private IndexerDataReader()
        {
            _connection = new OleDbConnection(ConnectionString);
        }
        #endregion

        #region [static]

        private static Lazy<IIndexerDataReader> _instance = new Lazy<IIndexerDataReader>(() => new IndexerDataReader());

        public static IIndexerDataReader Instance
        {
            get { return _instance.Value; }
        }

        #endregion

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

        public DataTable GetDataByReader(string query)
        {
            DataTable data = null;
            IDataReader reader = null;
            lock (_lock)
                try
                {
                    _connection.Open();
                    OleDbCommand cmd = new OleDbCommand(query, _connection);
                    reader = cmd.ExecuteReader();
                    data = GetDataTableFast(reader);
                }
                catch (Exception ex)
                {
                    WSUI.Core.Logger.WSSqlLogger.Instance.LogError("GetDataByReader: {0}", ex.Message);
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
            return new IndexerDataReader();
        }

        #region [private]
        private DataTable GetDataTableFromDataReader(IDataReader reader)
        {
            DataTable table = reader.GetSchemaTable();
            DataTable resultTable = new DataTable();
            foreach (DataRow dataRow in table.Rows)
            {
                DataColumn dataColumn = new DataColumn();
                dataColumn.ColumnName = dataRow["ColumnName"].ToString();
                dataColumn.DataType = Type.GetType(dataRow["DataType"].ToString());
                dataColumn.ReadOnly = (bool)dataRow["IsReadOnly"];
                dataColumn.AutoIncrement = (bool)dataRow["IsAutoIncrement"];
                dataColumn.Unique = (bool)dataRow["IsUnique"];

                resultTable.Columns.Add(dataColumn);
            }
            int row = 0;
            try
            {
                while (reader.Read())
                {
                    row++;
                    //bool res = false;
                    //try
                    //{
                    //    res = reader.Read();
                    //    if (!res)
                    //        break;
                    //}
                    //catch (InvalidCastException c)
                    //{
                    //    continue;
                    //}
                    //catch
                    //{
                    //    continue;
                    //}
                    
                    DataRow dataRow = resultTable.NewRow();
                   
                    for (int i = 0; i < resultTable.Columns.Count - 1; i++)
                    {
                        try
                        {
                            object val = reader[i];
                            dataRow[i] = val;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                            continue;
                        }
                    }
                    resultTable.Rows.Add(dataRow);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return resultTable;

        }

        private DataTable GetDataTableByAdapter(string query, OleDbConnection connection)
        {
            DataTable dt = new DataTable();
            new OleDbDataAdapter(query, connection).Fill(dt);
            return dt;
        }

        private DataTable GetDataTableFast(IDataReader rdr)
        {
            DataTable resultTable = GetDataTableFromDataReader(rdr);
            return resultTable;
        }

        #endregion

    }
}