///////////////////////////////////////////////////////////
//  BaseSearchSystem.cs
//  Implementation of the Class BaseSearchSystem
//  Generated by Enterprise Architect
//  Created on:      28-Sep-2013 3:55:40 PM
//  Original author: Yariki
///////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using WSUI.Core.Interfaces;
using WSUI.Core.Extensions;
using WSUI.Core.Logger;

namespace WSUI.Core.Core.Search 
{
	public abstract class BaseSearchSystem : ISearchSystem 
    {

		private Thread _mainSearchThread;
		private IList<ISearch> _listRules;
	    private volatile bool _needStop = false;

	    protected IList<ISystemSearchResult> InternalResult;
	    protected volatile bool _IsSearching = false;
        protected readonly object Lock1 = new object();
        protected readonly object Lock2 = new object();

		protected BaseSearchSystem()
        {
            _listRules = new List<ISearch>();
            InternalResult = new List<ISystemSearchResult>();
		}

		public virtual void Init()
		{
            _IsSearching = false;
            _listRules.ForEach(item => item.Init());
		}

		public virtual void Reset()
        {
            InternalResult.Clear();
            _IsSearching = false;
		    _needStop = false;
            _listRules.ForEach(item => item.Reset());
        }

		/// 
		/// <param name="searchCriteris"></param>
		public void SetSearchCriteria(string searchCriteris)
		{
		    _listRules.ForEach(item => item.SetSearchCriteria(searchCriteris));
            WSSqlLogger.Instance.LogInfo("Criteria: {0}", searchCriteris);
		}

		public virtual void Search()
		{
            if (_IsSearching)
		        return;
            InitBeforeSearch();
            _mainSearchThread = new Thread(DoSearch){Priority = ThreadPriority.Highest};
            _mainSearchThread.Start();
            RaiseSearchStarted();
		}

		public virtual void Stop()
		{
		    _needStop = true;
            _listRules.ForEach(item => item.Stop());
            RaiseSearchStopped();
		}

		public event Action<object> SearchStarted;

		public event Action<object> SearchFinished;

		public event Action<object> SearchStoped;

		public IList<ISystemSearchResult> GetResult()
        {
			return InternalResult;
		}

        public bool IsSearching { get { return _IsSearching; } }

	    protected virtual void RaiseSearchStarted()
		{
		    var temp = SearchStarted;
		    if (temp != null)
		        temp(null);
		}

		protected virtual void RaiseSearchFinished()
		{
		    var temp = SearchFinished;
		    if (temp != null)
		        temp(null);
		}

		protected virtual void RaiseSearchStopped()
		{
		    var temp = SearchStoped;
		    if (temp != null)
		        temp(null);
		}

		protected virtual void ProcessData()
        {

		}

		protected IEnumerable<ISearch> GetRules()
        {
			return _listRules;
		}

		protected virtual void DoSearch()
		{
		    try
		    {
		        var watch = new Stopwatch();
                watch.Start();
                var events = _listRules.Select(item => item.GetEvent()).ToArray();
                if (events == null || events.Length == 0)
		        {
                    WSSqlLogger.Instance.LogInfo("List of Events is empty");
                    return;
		        }
                _listRules.ForEach(item => item.Search());
		        WaitHandle.WaitAll(events);
                WSSqlLogger.Instance.LogInfo("+++++++++++++++++ searching is DONE!!!!+++++++++++++++");
		        if (_needStop)
		        {
		            RaiseSearchStopped();
                    _IsSearching = false;
                    WSSqlLogger.Instance.LogError("Searching was stoped");
                    return;
		        }
                ProcessData();
		        foreach (var item in _listRules.OrderBy(i => i.Priority))
		        {
		            var result = (item as ISearchRule).GetResults();
		            if (result == null)
		                continue;
		            var itemResult = new SystemSearchResult(item.Priority, result.OperationResult);
		            InternalResult.Add(itemResult);
		        }
                watch.Stop();
                WSSqlLogger.Instance.LogInfo("BaseSearchSystem: {0}",watch.ElapsedMilliseconds);
		    }
		    catch (Exception ex)
		    {
                WSSqlLogger.Instance.LogError("{0}",ex.Message);
		    }
		    finally
		    {
                _IsSearching = false;
                RaiseSearchFinished();
		    }
		}

	    protected void AddRule(ISearch rule)
	    {
	        if (rule == null)
	            return;
            _listRules.Add(rule);
	    }

	    protected virtual void InitBeforeSearch()
	    {
            if(InternalResult.Count > 0)
	            InternalResult.Clear(); 
	    }

	}//end BaseSearchSystem

}//end namespace Search