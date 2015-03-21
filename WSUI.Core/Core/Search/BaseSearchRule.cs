///////////////////////////////////////////////////////////
//  BaseSearchRule.cs
//  Implementation of the Class BaseSearchRule
//  Generated by Enterprise Architect
//  Created on:      27-Sep-2013 11:34:46 PM
//  Original author: Yariki
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nest;
using WSUI.Core.Core.AdvancedSearchCriteria;
using WSUI.Core.Core.ElasticSearch;
using WSUI.Core.Core.Rules;
using WSUI.Core.Data.ElasticSearch;
using WSUI.Core.Enums;
using WSUI.Core.Interfaces;
using WSUI.Core.Logger;
using WSUI.Core.Utils;

namespace WSUI.Core.Core.Search
{
    public abstract class BaseSearchRule<T, E> : ISearch, ISearchRule where T : class, ISearchObject, new() where E : class, IElasticSearchObject
    {
        #region [needs private]

        private Thread _ruleThread;
        private IQueryReader _reader;
        private volatile bool _isSearching = false;
        private bool _exludeIgnored = false;
        private WSUIElasticSearchClient _elasticSearchClient;
        private Func<T> _create; 

        #endregion [needs private]

        #region [needs protected]

        protected volatile bool IsInterupt = false;
        protected IList<T> Result;
        protected bool NeedInterrup;
        protected DateTime LastDate;
        protected IEnumerable<IAdvancedSearchCriteria> AdvancedSearchCriterias; 
        protected string Query;
        protected AutoResetEvent Event;
        protected int TopQueryResult = 100;
        protected int CountFirstProcess = 35;
        protected int CountSecondProcess = 7;
        protected int CountAdded = 0;
        protected int CountProcess = 0;
        protected volatile bool NeedStop = false;
        protected volatile object Lock = null;
        

        private volatile object _internalLock = new object();

        // results
        private TypeResult _typeResult;

        private IList<IResultMessage> _listMessage;

        protected string QueryAnd = " AND \"{0}*\"";

        protected string RuleName;

        #endregion [needs protected]

        protected BaseSearchRule()
            : this(null, false)
        {
          
        }

        protected BaseSearchRule(object lockObject, bool exludeIgnored)
        {
            Event = new AutoResetEvent(false);
            Result = new List<T>();
            Lock = lockObject ?? new object();
            _exludeIgnored = exludeIgnored;
            _elasticSearchClient = new WSUIElasticSearchClient();
            _create = New<T>.Instance;
        }

        public ISearchResult GetResults()
        {
            return new SearchResult(_typeResult, _listMessage.ToList(), (IEnumerable<ISearchObject>)Result);
        }

        /// <summary>
        /// @param ="criteris"
        /// </summary>
        /// <param name="criteria"></param>
        public virtual void SetSearchCriteria(string criteria)
        {
            Query = criteria.Trim();
        }

        public virtual void SetAdvancedSearchCriteria(IEnumerable<IAdvancedSearchCriteria> advancedSearchCriterias)
        {
            AdvancedSearchCriterias = advancedSearchCriterias;
        }

        public void Search()
        {
            if (_isSearching)
            {
                Event.Set();
                return;
            }
            InitBeforeSearching();
            _ruleThread = new Thread(DoQuery) { Priority = ThreadPriority.Highest, Name = string.Format("{0}({1})", this.GetType().Name, typeof(T).Name) };
            _ruleThread.Start();
        }

        public virtual void Stop()
        {
            NeedStop = true;
        }

        protected virtual void DoQuery()
        {
            try
            {
                _isSearching = true;
                string query = Query; //QueryGenerator.Instance.GenerateQuery(typeof(T), Query, TopQueryResult, this,IsAdvancedMode);
                if (string.IsNullOrEmpty(query))
                    throw new ArgumentNullException("Query is null or empty");
                WSSqlLogger.Instance.LogInfo("Query<{0}>: {1}", typeof(T).Name, query);
                
                Stopwatch watch = new Stopwatch();
                watch.Start();

                
                var result =  NeedSorting 
                    ? _elasticSearchClient.ElasticClient.Search<E>(s => s
                    .From(0)
                    .Size(TopQueryResult)
                    .Query(BuildQuery)
                    .Sort(BuildSortSelector)
                    )
                    : _elasticSearchClient.ElasticClient.Search<E>(s => s
                    .From(0)
                    .Size(TopQueryResult)
                    .Query(BuildQuery)
                    ) 
                    ;
                watch.Stop();

                // additional process
                if (!NeedStop && result != null && result.Documents.Any())
                {
                    ReadDataFromTable(result.Documents);
                    ProcessResult();
                    _typeResult = TypeResult.Ok;
                    
                }
                else
                {
                    _typeResult = TypeResult.Error;
                    _listMessage.Add(new ResultMessage() { Message = "Rule was stoped" });
                }
            }
            catch (Exception ex)
            {
                _typeResult = TypeResult.Error;
                _listMessage.Add(new ResultMessage() { Message = ex.Message });
                WSSqlLogger.Instance.LogError("Search <{0}>: {1}", typeof(T).Name, ex.Message);
            }
            finally
            {
                CountAdded = 0;
                TopQueryResult = CountProcess = CountSecondProcess;
                _isSearching = false;
                NeedStop = false;
                IsInterupt = false;
                IsInit = false;
                Event.Set();
            }
        }

        protected virtual IFieldSort BuildSortSelector(SortFieldDescriptor<E> sortFieldDescriptor)
        {
            return default(IFieldSort);
        }

        protected virtual QueryContainer BuildQuery(QueryDescriptor<E> queryDescriptor)
        {
            return default (QueryContainer);
        }

        protected virtual DataTable GetDataTable(string query)
        {
            return IndexerDataReader.Instance.GetDataByAdapter(query);
        }

        protected bool IsInit { get; private set; }

        protected virtual void ReadDataFromTable(IEnumerable<E> data)
        {
            Parallel.ForEach(data.AsEnumerable(), ReadData);
        }

        ///
        /// <param name="reader"></param>
        private void ReadData(E searchObject)
        {
            var result = _create.Invoke() as T;
            if (result == null)
                return;
            result.SetDataObject(searchObject);
            lock (_internalLock)
            {
                Result.Add(result);
                ProcessCountAdded();    
            }
        }

        protected virtual void ProcessCountAdded()
        {
            CountAdded++;
            if (CountAdded == CountProcess)
                IsInterupt = true;
        }

        public AutoResetEvent GetEvent()
        {
            Event.Reset();
            return Event;
        }

        public virtual void Reset()
        {
            CountAdded = 0;
            CountProcess = 0;
            TopQueryResult = CountProcess = CountFirstProcess;
            Query = string.Empty;
            LastDate = GetCurrentDateTime();
            IsInterupt = false;
            NeedStop = false;
            IsInit = true;
            Result.Clear();
        }

        public bool IsSearching { get { return _isSearching; } }

        public int Priority { get; protected set; }

        public RuleObjectType ObjectType { get; protected set; }

        protected virtual void ProcessResult()
        {
        }

        protected virtual bool NeedSorting
        {
            get { return true; }
        }

        public virtual void Init()
        {
            LastDate = GetCurrentDateTime();
            TopQueryResult = CountProcess = CountFirstProcess;
            CountAdded = 0;
            _typeResult = TypeResult.None;
            _listMessage = new List<IResultMessage>();
            IsInit = true;
        }

        public void SetProcessingRecordCount(int first, int second)
        {
            TopQueryResult = CountProcess = CountFirstProcess = first;
            CountSecondProcess = second;
        }

        public bool IsAdvancedMode { get; set; }
        public bool IncludedInAdvancedMode { get { return GetIncludedInAdvancedMode(); } }

        protected virtual bool GetIncludedInAdvancedMode()
        {
            return false;
        }

        protected virtual string OnGenerateAdvancedWherePart()
        {
            return string.Empty;
        }

        protected DateTime GetCurrentDateTime()
        {
            return DateTime.Now.AddDays(1);
        }

        protected string FormatDate(ref DateTime date)
        {
            return date.ToString("yyyy/MM/dd hh:mm:ss").Replace('.', '/');
        }

        protected List<string> GetProcessingSearchCriteria(string keyword = "")
        {
            IList<IRule> listRuleCriteriasRules = RuleFactory.Instance.GetAllRules();
            var tempCriteria =  string.IsNullOrEmpty(keyword) ? Query : keyword;
            var listW = new List<string>();

            foreach (var rule in listRuleCriteriasRules.OrderBy(i => i.Priority))
            {
                listW.AddRange(rule.ApplyRule(tempCriteria));
                tempCriteria = rule.ClearCriteriaAccordingRule(tempCriteria);
            }
            return listW;
        }

        protected virtual void InitBeforeSearching()
        {
            _typeResult = TypeResult.None;
            _listMessage.Clear();
            Result.Clear();
        }

    }//end BaseSearchRule
}//end namespace Search