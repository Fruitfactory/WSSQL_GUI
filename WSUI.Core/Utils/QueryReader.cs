///////////////////////////////////////////////////////////
//  QueryReader.cs
//  Implementation of the Class QueryReader
//  Generated by Enterprise Architect
//  Created on:      29-Sep-2013 9:24:58 PM
//  Original author: Yariki
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using OF.Core.Interfaces;
using OF.Core.Logger;

namespace OF.Core.Utils
{
    // this approach really create objets must faster then Activator.CreateInstance
    // for example 10 000 000 objects:
    // Activator.CreateInstance - 2774 ms
    // New<T> - 1425 ms
    internal static class New<T> where T : new()
    {
        public static readonly Func<T> Instance = Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();
    }

    /// <summary>
    /// we don't uset it from this moment. Deprecated. Should delete this file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryReader<T> : IQueryReader where T : new()
    {
        #region [needs]

        private IList<Tuple<string, string, int, bool>> _intermalList;
        private Func<T> _create;
        private object _lock = new object();

        #endregion [needs]

        private QueryReader(IList<Tuple<string, string, int, bool>> fields)
        {
            _intermalList = fields;
            _create = New<T>.Instance;
        }

        public static QueryReader<T1> CreateNewReader<T1>(IList<Tuple<string, string, int, bool>> listFields) where T1 : new()
        {
            return new QueryReader<T1>(listFields);
        }

        ///
        /// <param name="reader"></param>
        /// <param name="type"></param>
        public object ReadResult(IDataReader reader)
        {
            var result = _create.Invoke() as ISearchObject;
            if (result == null)
                return null;
            Parallel.ForEach(_intermalList, tuple =>
            {
                try
                {
                    int index = tuple.Item3 - 1;
                    if (index >= reader.FieldCount)
                        return;
                    object val = null;
                    lock (_lock)
                        val = reader[index];
                    if (DBNull.Value.Equals(val))
                        return;
                    //result.SetValue(tuple.Item3, val);
                }
                catch (Exception ex)
                {
                    WSSqlLogger.Instance.LogError("Readresult: {0}", ex.Message);
                }
            });
            return result;
        }

        public object ReadResult(DataRow row)
        {
            var result = _create.Invoke() as ISearchObject;
            if (result == null)
                return null;
            var count = row.Table.Columns.Count;
            Parallel.ForEach(_intermalList, tuple =>
            {
                try
                {
                    int index = tuple.Item3 - 1;
                    if (index >= count)
                        return;
                    object val = null;
                    lock (_lock)
                        val = row[index];
                    if (DBNull.Value.Equals(val))
                        return;
                    //result.SetValue(tuple.Item3, val);
                }
                catch (Exception ex)
                {
                    WSSqlLogger.Instance.LogError("Readresult: {0}", ex.Message);
                }
            });
            return result;
        }
    }//end QueryReader
}//end namespace Utils