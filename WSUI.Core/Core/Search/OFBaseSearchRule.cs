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
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Outlook;
using Microsoft.Practices.Unity;
using Nest;
using Newtonsoft.Json;
using OF.Core.Core.AdvancedSearchCriteria;
using OF.Core.Core.Attributes;
using OF.Core.Core.ElasticSearch;
using OF.Core.Core.Rules;
using OF.Core.Data.ElasticSearch;
using OF.Core.Data.ElasticSearch.Request;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Core.Logger;
using OF.Core.Utils;
using Exception = System.Exception;

namespace OF.Core.Core.Search
{
    public abstract class BaseSearchRule<T, E> : ISearch, ISearchRule
        where T : class, ISearchObject, new()
        where E : class, IElasticSearchObject, new()
    {
        #region [needs private]

        private Thread _ruleThread;
        private volatile bool _isSearching = false;
        private bool _exludeIgnored = false;
        private IElasticSearchClient _elasticSearchClient;
        private Func<T> _create;
        private int _from = 0;
        private long _total = 0;
        private List<OFRuleToken> _keywords;
        private IUnityContainer _unityContainer;

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
        protected volatile bool NeedStop = false;
        protected volatile object Lock = null;



        private volatile object _internalLock = new object();

        // results
        private OFTypeResult _typeResult;

        private IList<IResultMessage> _listMessage;

        protected string QueryAnd = " AND \"{0}*\"";

        protected string RuleName;

        #endregion [needs protected]

        protected BaseSearchRule(IUnityContainer unityContainer)
            : this(null, false,unityContainer)
        {

        }

        protected BaseSearchRule(object lockObject, bool exludeIgnored, IUnityContainer unityContainer)
        {
            Event = new AutoResetEvent(false);
            Result = new List<T>();
            Lock = lockObject ?? new object();
            _exludeIgnored = exludeIgnored;
            _unityContainer = unityContainer;
            _elasticSearchClient = _unityContainer.Resolve<IElasticSearchClient>();
            _create = New<T>.Instance;
        }

        public ISearchResult GetResults()
        {
            return new OFSearchResult(_typeResult, _listMessage.ToList(), (IEnumerable<ISearchObject>)Result);
        }

        /// <summary>
        /// @param ="criteris"
        /// </summary>
        /// <param name="crieria"></param>
        public virtual void SetSearchCriteria(string criteria)
        {
            Query = criteria.ToLowerInvariant().Trim();
            _keywords = GetProcessingSearchCriteria();
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
                string query = Query;
                if (string.IsNullOrEmpty(query))
                    throw new ArgumentNullException("Query is null or empty");
                if (_total != 0 && _from >= _total)
                {
                    return;
                }

                IEnumerable<E> Documents = null;
                long total = 0;
                int took = 0;

                if (IsAdvancedMode)
                {
                    ISearchResponse<E> resultAdvance = _elasticSearchClient.ElasticClient.Search<E>(s => s
                        .From(_from)
                        .Size(TopQueryResult)
                        .Query(BuildAdvancedQuery)
                        .Sort(BuildAdvancedFieldSortSortSelector)
                        );
                    if (resultAdvance.IsNotNull())
                    {
#if DEBUG
                        var str = Encoding.Default.GetString(resultAdvance.RequestInformation.Request);
#endif
                        Documents = resultAdvance.Documents;
                        total = resultAdvance.Total;
                    }

                }
                else
                {
                    IRawSearchResult<E> result = null;
                    var body = GetSearchBody();
                    if (body.IsNotNull())
                    {
                        result = GetMainResult(body, result);
                        if (result.IsNotNull() && result.Documents.Any())
                        {
                            Documents = result.Documents;
                            took = result.Took;
                            total = result.Total;
                        }
                        else
                        {
                            Documents = GetAlternativeResult(out took, out total);
                        }
                    }
                }

                // additional process
                if (!NeedStop && Documents.Any())
                {
                    _total = total;
                    _from += Documents.Count() == TopQueryResult ? TopQueryResult : Documents.Count();
                    ReadDataFromTable(Documents);
                    ProcessResult();
                    _typeResult = OFTypeResult.Ok;
                }
                else
                {
                    _typeResult = OFTypeResult.None;
                    _listMessage.Add(new OFResultMessage() {Message = "Rule was stoped"});
                }
            }
            catch (OutOfMemoryException mex)
            {
                _typeResult = OFTypeResult.Error;
                _listMessage.Add(new OFResultMessage() { Message = "Please, check the memory usage on ElasticSearch side. See log for details." });
                OFLogger.Instance.LogError("Search <{0}>: {1}", typeof(T).Name, mex.Message);
            }
            catch (Exception ex)
            {
                _typeResult = OFTypeResult.Error;
                _listMessage.Add(new OFResultMessage() { Message = "An error was occured during searching.Please, see log." });
                OFLogger.Instance.LogError("Search <{0}>: {1}", typeof(T).Name, ex.ToString());
            }
            finally
            {
                CountAdded = 0;
                TopQueryResult = CountSecondProcess;
                _isSearching = false;
                NeedStop = false;
                IsInterupt = false;
                IsInit = false;
                Event.Set();
            }
        }

        private IEnumerable<E> GetAlternativeResult(out int took, out long total)
        {
            IEnumerable<E> Documents = new List<E>();
            OFBody body;
            IRawSearchResult<E> result;
            body = GetAlternativeSearchBody();
            took = 0;
            total = 0;
            if (body.IsNotNull())
            {
                body.from = _from;
                body.size = TopQueryResult;
                result = _elasticSearchClient.RawSearch<E>(body);
                if (result.IsNotNull())
                {
                    Documents = result.Documents;
                    took = result.Took;
                    total = result.Total;
                }
            }
            return Documents;
        }

        private IRawSearchResult<E> GetMainResult(OFBody body, IRawSearchResult<E> result)
        {
            body.from = _from;
            body.size = TopQueryResult;
            result = _elasticSearchClient.RawSearch<E>(body);
            return result;
        }

        protected virtual IFieldSort BuildSortSelector(SortFieldDescriptor<E> sortFieldDescriptor)
        {
            return default(IFieldSort);
        }

        protected virtual IFieldSort BuildAdvancedFieldSortSortSelector(SortFieldDescriptor<E> sortFieldDescriptor)
        {
            return default(IFieldSort);
        }

        protected virtual QueryContainer BuildAdvancedQuery(QueryDescriptor<E> queryDescriptor)
        {
            return default(QueryContainer);
        }

        protected virtual OFBody GetSearchBody()
        {
            return null;
        }
        
        protected virtual OFBody GetAlternativeSearchBody()
        {
            return null;
        }

        protected virtual IEnumerable<string> GetRequiredFields()
        {
            var requiredProperties = GetNotIgnoredProperties(typeof (E));
            return requiredProperties.Select(p => p.Name.ToLowerInvariant());
        }

        private PropertyInfo[] GetNotIgnoredProperties(Type type)
        {
            var arr = type.GetProperties().Where(p => !IsIgnoredProperty(p)).ToArray();
            return arr;
        }

        private bool IsIgnoredProperty(PropertyInfo pInfo)
        {
            var attributes = pInfo.GetCustomAttributes(typeof(OFIgnoreAttribute), true);
            return attributes != null && attributes.Length > 0;
        }

        protected virtual DataTable GetDataTable(string query)
        {
            return OFIndexerDataReader.Instance.GetDataByAdapter(query);
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
        }

        public AutoResetEvent GetEvent()
        {
            Event.Reset();
            return Event;
        }

        public virtual void Reset()
        {
            CountAdded = 0;
            _from = 0;
            _total = 0;
            TopQueryResult = CountFirstProcess;
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
            _from = 0;
            _total = 0;
            LastDate = GetCurrentDateTime();
            TopQueryResult = CountFirstProcess;
            CountAdded = 0;
            _typeResult = OFTypeResult.None;
            _listMessage = new List<IResultMessage>();
            IsInit = true;
        }

        public void SetProcessingRecordCount(int first, int second)
        {
            TopQueryResult = CountFirstProcess = first;
            CountSecondProcess = second;
        }

        public bool IsAdvancedMode { get; set; }
        public bool IncludedInAdvancedMode { get { return GetIncludedInAdvancedMode(); } }

        protected virtual bool GetIncludedInAdvancedMode()
        {
            return false;
        }

        protected DateTime GetCurrentDateTime()
        {
            return DateTime.Now.AddDays(1);
        }

        protected IList<OFRuleToken> GetKeywordsList()
        {
            return _keywords;
        }

        protected string FormatDate(ref DateTime date)
        {
            return date.ToString("yyyy/MM/dd hh:mm:ss").Replace('.', '/');
        }

        protected string FormatCriteria(string criteria)
        {
            return string.Format("{0}*", criteria);
        }

        protected List<OFRuleToken> GetProcessingSearchCriteria(string keyword = "")
        {
            IList<IRule> listRuleCriteriasRules = OFRuleFactory.Instance.GetAllRules();
            var tempCriteria = string.IsNullOrEmpty(keyword) ? Query : keyword;
            var listW = new List<OFRuleToken>();

            foreach (var rule in listRuleCriteriasRules.OrderBy(i => i.Priority))
            {
                var result = rule.ApplyRule(tempCriteria);
                if (result.IsNotNull())
                {
                    listW.AddRange(result);
                    tempCriteria = rule.ClearCriteriaAccordingRule(tempCriteria);
                }
            }
            return listW;
        }

        protected virtual void InitBeforeSearching()
        {
            _typeResult = OFTypeResult.None;
            _listMessage.Clear();
            Result.Clear();
        }

        protected void SetTypeResult(OFTypeResult type)
        {
            _typeResult = type;
        }

        protected void AddMessage(string message)
        {
            if (_listMessage != null)
            {
                _listMessage.Add(new OFResultMessage(){Message = message});
            }
        }

    }//end BaseSearchRule
}//end namespace Search