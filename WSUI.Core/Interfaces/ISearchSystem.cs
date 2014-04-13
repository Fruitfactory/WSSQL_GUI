///////////////////////////////////////////////////////////
//  ISearchSystem.cs
//  Implementation of the Interface ISearchSystem
//  Generated by Enterprise Architect
//  Created on:      28-Sep-2013 3:27:14 PM
//  Original author: Yariki
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace WSUI.Core.Interfaces
{
    public interface ISearchSystem
    {
        void Init();

        void Reset();

        ///
        /// <param name="searchCriteris"></param>
        void SetSearchCriteria(string searchCriteris);

        void Search();

        void Stop();

        event Action<object> SearchStarted;

        event Action<object> SearchFinished;

        event Action<object> SearchStoped;

        IList<ISystemSearchResult> GetResult();

        bool IsSearching { get; }
    }//end ISearchSystem
}//end namespace Interfaces