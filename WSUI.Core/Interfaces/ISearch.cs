///////////////////////////////////////////////////////////
//  ISearch.cs
//  Implementation of the Interface ISearch
//  Generated by Enterprise Architect
//  Created on:      27-Sep-2013 11:16:12 PM
//  Original author: Yariki
///////////////////////////////////////////////////////////


using System;
using System.Threading;

namespace WSUI.Core.Interfaces {
	public interface ISearch       {
		/// <summary>
		/// @param ="criteris"
		/// </summary>
		/// <param name="criteria"></param>
		void SetSearchCriteria(string criteria);

		void Search();

		void Stop();

		AutoResetEvent GetEvent();

		void Reset();

		bool IsSearching{
			get;
		}

		int Priority{
			get;
		}

		void Init();
	    void SetProcessingRecordCount(int first, int second);

	}//end ISearch

}//end namespace Interfaces