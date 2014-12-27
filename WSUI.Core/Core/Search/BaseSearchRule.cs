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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WSUI.Core.Core.AdvancedSearchCriteria;
using WSUI.Core.Core.Rules;
using WSUI.Core.Enums;
using WSUI.Core.Interfaces;
using WSUI.Core.Logger;
using WSUI.Core.Utils;

namespace WSUI.Core.Core.Search
{
    public abstract class BaseSearchRule<T> : ISearch, ISearchRule, IRuleQueryGenerator where T : class, new()
    {
        #region [needs private]

        private Thread _ruleThread;
        private IQueryReader _reader;
        private volatile bool _isSearching = false;
        private bool _exludeIgnored = false;

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

        protected volatile object InternalLock = new object();

        // results
        private TypeResult _typeResult;

        private IList<IResultMessage> _listMessage;

        protected string QueryAnd = " AND \"{0}\"";

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
                string query = QueryGenerator.Instance.GenerateQuery(typeof(T), Query, TopQueryResult, this,IsAdvancedMode);
                if (string.IsNullOrEmpty(query))
                    throw new ArgumentNullException("Query is null or empty");
                WSSqlLogger.Instance.LogInfo("Query<{0}>: {1}", typeof(T).Name, query);
                
                Stopwatch watch = new Stopwatch();
                watch.Start();

                DataTable resultTable = GetDataTable(query);

                watch.Stop();
                WSSqlLogger.Instance.LogInfo("GetDataTable: {0}, {1}",typeof(T).Name,watch.ElapsedMilliseconds);

                // additional process
                if (!NeedStop && resultTable != null)
                {
                    ReadDataFromTable(resultTable);
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
                Event.Set();
            }
        }

        protected virtual DataTable GetDataTable(string query)
        {
            return IndexerDataReader.Instance.GetDataByAdapter(query);
        }

        protected virtual void ReadDataFromTable(DataTable data)
        {
            if (data.Rows.Count == 0)
                return;
            Parallel.ForEach(data.AsEnumerable(), ReadData);
        }

        ///
        /// <param name="reader"></param>
        private void ReadData(IDataReader reader)
        {
            var result = Reader.ReadResult(reader) as T;
            if (result == null)
                return;
            Result.Add(result);
            ProcessCountAdded();
        }

        private void ReadData(DataRow row)
        {
            var result = Reader.ReadResult(row) as T;
            if (result == null)
                return;
            lock (InternalLock)
                Result.Add(result);
            ProcessCountAdded();
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
            Result.Clear();
        }

        public bool IsSearching { get { return _isSearching; } }

        public int Priority { get; protected set; }

        public RuleObjectType ObjectType { get; protected set; }

        protected virtual void ProcessResult()
        {
        }

        public virtual void Init()
        {
            LastDate = GetCurrentDateTime();
            TopQueryResult = CountProcess = CountFirstProcess;
            CountAdded = 0;
            _typeResult = TypeResult.None;
            _listMessage = new List<IResultMessage>();
        }

        public void SetProcessingRecordCount(int first, int second)
        {
            TopQueryResult = CountProcess = CountFirstProcess = first;
            CountSecondProcess = second;
        }

        public bool IsAdvancedMode { get; set; }
        public bool IncludedInAdvancedMode { get { return GetIncludedInAdvancedMode(); } }

        public string GenerateWherePart(IList<IRule> listCriteriaRules)
        {
            return OnGenerateWherePart(listCriteriaRules);
        }

        public string GenerateAdvancedWherePart()
        {
            return OnGenerateAdvancedWherePart();
        }

        protected virtual string OnGenerateWherePart(IList<IRule> listCriterisRules)
        {
            return string.Empty;
        }

        protected virtual bool GetIncludedInAdvancedMode()
        {
            return false;
        }

        protected virtual string OnGenerateAdvancedWherePart()
        {
            return string.Empty;
        }

        protected IQueryReader Reader
        {
            get { return _reader ?? (_reader = QueryReader<T>.CreateNewReader<T>(FieldCash.Instance.GetFields(typeof(T)))); }
        }

        protected DateTime GetCurrentDateTime()
        {
            return DateTime.Now.AddDays(1);
        }

        protected string FormatDate(ref DateTime date)
        {
            return date.ToString("yyyy/MM/dd hh:mm:ss").Replace('.', '/');
        }

        protected Tuple<string, List<string>> GetProcessingSearchCriteria(IList<IRule> listRuleCriteriasRules)
        {
            var tempCriteria = Query;
            var andClause = string.Empty;
            var listW = new List<string>();

            foreach (var rule in listRuleCriteriasRules.OrderBy(i => i.Priority))
            {
                listW.AddRange(rule.ApplyRule(tempCriteria));
                tempCriteria = rule.ClearCriteriaAccordingRule(tempCriteria);
            }

            if (listW.Count > 1)
            {
                StringBuilder temp = new StringBuilder();
                temp.Append(string.Format("'\"{0}*\"'", listW[0]));
                for (int i = 1; i < listW.Count; i++)
                {
                    temp.Append(string.Format(QueryAnd, listW[i]));
                }
                andClause = temp.ToString() + "'";
            }
            else
            {
                andClause = string.Format("'\"{0}*\"'", listW[0]);
            }
            return new Tuple<string, List<string>>(andClause, listW.ToList());
        }

        protected virtual void InitBeforeSearching()
        {
            _typeResult = TypeResult.None;
            _listMessage.Clear();
            Result.Clear();
        }

    }//end BaseSearchRule
}//end namespace Search