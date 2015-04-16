///////////////////////////////////////////////////////////
//  IQueryGenerator.cs
//  Implementation of the Interface IQueryGenerator
//  Generated by Enterprise Architect
//  Created on:      28-Sep-2013 1:56:08 PM
//  Original author: Yariki
///////////////////////////////////////////////////////////

using System;

namespace OF.Core.Interfaces
{
    public interface IQueryGenerator
    {
        ///
        /// <param name="type">type which use for generating query</param>
        /// <param name="searchCriteria">search string</param>
        /// <param name="topResult"></param>
        /// <param name="exludeIgnored">should we ignore property</param>
        string GenerateQuery(Type type, string searchCriteria, int topResult, IRuleQueryGenerator ruleQueryGenerator, bool isAdvancedMode);
    }//end IQueryGenerator
}//end namespace Interfaces