///////////////////////////////////////////////////////////
//  QueryReader.cs
//  Implementation of the Class QueryReader
//  Generated by Enterprise Architect
//  Created on:      29-Sep-2013 9:24:58 PM
//  Original author: Yariki
///////////////////////////////////////////////////////////


using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using WSUI.Core.Interfaces;

namespace WSUI.Core.Utils 
{
	public class QueryReader : IQueryReader
    {
        #region [needs]

	    private Type _type;
	    private IList<Tuple<string, string, int, bool>> _intermalList; 

        #endregion

        private QueryReader(Type type,IList<Tuple<string, string, int, bool>> fields )
        {
            _type = type;
            _intermalList = fields;
        }

	    public static QueryReader CreateNewReader(Type type,IList<Tuple<string, string, int, bool>> listFields )
	    {
	        return new QueryReader(type,listFields); 
	    }

		/// 
		/// <param name="reader"></param>
		/// <param name="type"></param>
		public object ReadResult(IDataReader reader)
		{
		    var result = Activator.CreateInstance(_type) as ISearchObject;
		    if (result == null)
		        return null;

		    foreach (var tuple in _intermalList)
		    {
		        int index = tuple.Item3 - 1;
		        if (index >= reader.FieldCount)
		            break;
		        object val = reader[index];
                if(DBNull.Value.Equals(val))
                    continue;
                result.SetValue(tuple.Item3,val);
		    }
			return result;
		}

	}//end QueryReader

}//end namespace Utils