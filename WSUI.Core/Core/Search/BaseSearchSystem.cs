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
using Microsoft.Practices.Unity;
using OF.Core.Core.AdvancedSearchCriteria;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Core.Logger;
using Exception = System.Exception;

namespace OF.Core.Core.Search
{
    public abstract class BaseSearchSystem : ISearchSystem
    {
        private Thread _mainSearchThread;
        private IList<ISearch> _listRules;
        
        private bool _isAdvancedMode = false;

        protected IList<ISystemSearchResult> InternalResult;
        protected volatile bool _IsSearching = false;
        protected readonly object Lock1 = new object();
        protected readonly object Lock2 = new object();
        protected volatile bool _needStop = false;

        protected BaseSearchSystem()
        {
            _listRules = new List<ISearch>();
            InternalResult = new List<ISystemSearchResult>();
        }

        protected BaseSearchSystem(object Lock)
            : this()
        {
            Lock1 = Lock;
        }


        public virtual void Init(IUnityContainer container)
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
            OFLogger.Instance.LogDebug("Criteria: {0}", searchCriteris);
        }

        public void SetAdvancedSearchCriterias(IEnumerable<IAdvancedSearchCriteria> advancedSearchCriterias)
        {
            _listRules.ForEach(item => item.SetAdvancedSearchCriteria(advancedSearchCriterias));
        }

        public virtual void Search()
        {
            if (_IsSearching)
                return;
            InitBeforeSearch();
            _mainSearchThread = new Thread(DoSearch) { Priority = ThreadPriority.Highest, Name = "MainSearchSystem" };
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
        public bool IsAdvancedMode
        {
            get { return _isAdvancedMode; }
            set
            {
                _isAdvancedMode = value;
                GetRules().ForEach(r => r.IsAdvancedMode = value);
            }
        }

        public virtual void SetProcessingRecordCount(int first, int second)
        {
            GetRules().ForEach(r => r.SetProcessingRecordCount(first, second));
        }

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
                var events = IsAdvancedMode ? _listRules.Where(item => item.IncludedInAdvancedMode).Select(item => item.GetEvent()).ToArray() : _listRules.Select(item => item.GetEvent()).ToArray();
                if (events == null || events.Length == 0)
                {
                    OFLogger.Instance.LogDebug("List of Events is empty");
                    return;
                }
                var watchSearch = new Stopwatch();
                watchSearch.Start();

                if (IsAdvancedMode)
                    _listRules.Where(item => item.IncludedInAdvancedMode).ForEach(item => item.Search());
                else 
                    _listRules.ForEach(item => item.Search());

                WaitHandle.WaitAll(events);
                watchSearch.Stop();
                OFLogger.Instance.LogDebug("------------------- searching is DONE!!!!--------------------- {0}ms",watchSearch.ElapsedMilliseconds);//

                if (_needStop)
                {
                    RaiseSearchStopped();
                    _IsSearching = false;
                    OFLogger.Instance.LogError("Searching was stoped");
                    return;
                }
                ProcessData();
                var items = IsAdvancedMode
                    ? _listRules.Where(item => item.IncludedInAdvancedMode).OrderBy(i => i.Priority)
                    : _listRules.OrderBy(i => i.Priority);
                foreach (var item in items)
                {
                    var result = (item as ISearchRule).GetResults();
                    if (result == null)
                        continue;
                    var itemResult = new SystemSearchResult(item.Priority, result, item.ObjectType);
                    InternalResult.Add(itemResult);
                }
                watch.Stop();
                OFLogger.Instance.LogDebug("BaseSearchSystem: {0}", watch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError("{0}", ex.Message);
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
            if (InternalResult.Count > 0)
                InternalResult.Clear();
        }
    }//end BaseSearchSystem
}//end namespace Search