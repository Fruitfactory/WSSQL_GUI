///////////////////////////////////////////////////////////
//  ISearchResult.cs
//  Implementation of the Interface ISearchResult
//  Generated by Enterprise Architect
//  Created on:      28-Sep-2013 2:32:18 PM
//  Original author: Yariki
///////////////////////////////////////////////////////////

using System.Collections.Generic;
using OF.Core.Enums;

namespace OF.Core.Interfaces
{
    public interface ISearchResult
    {
        OFTypeResult Type { get; }

        IList<IResultMessage> Messages { get; }

        IList<ISearchObject> OperationResult { get; }
    }//end ISearchResult
}//end namespace Interfaces