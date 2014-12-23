///////////////////////////////////////////////////////////
//  IRuleQueryGenerator.cs
//  Implementation of the Interface IRuleQueryGenerator
//  Generated by Enterprise Architect
//  Created on:      29-Sep-2013 10:48:56 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////

using System.Collections.Generic;
using WSUI.Core.Core.Rules;

namespace WSUI.Core.Interfaces
{
    public interface IRuleQueryGenerator
    {
        ///
        /// <param name="listCriteriaRules"></param>
        string GenerateWherePart(IList<IRule> listCriteriaRules);

        string GenerateAdvancedWherePart(string advancedCriteria);
    }//end IRuleQueryGenerator
}//end namespace Interfaces