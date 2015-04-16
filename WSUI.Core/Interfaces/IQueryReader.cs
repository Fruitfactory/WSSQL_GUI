///////////////////////////////////////////////////////////
//  IQueryReader.cs
//  Implementation of the Interface IQueryReader
//  Generated by Enterprise Architect
//  Created on:      29-Sep-2013 9:24:48 PM
//  Original author: Yariki
///////////////////////////////////////////////////////////

using System.Data;

namespace OF.Core.Interfaces
{
    public interface IQueryReader
    {
        ///
        /// <param name="reader"></param>
        /// <param name="type"></param>
        object ReadResult(IDataReader reader);

        object ReadResult(DataRow row);
    }//end IQueryReader
}//end namespace Interfaces