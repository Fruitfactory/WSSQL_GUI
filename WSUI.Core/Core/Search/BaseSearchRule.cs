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
using System.Data.Common;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using WSUI.Core.Data;
using WSUI.Core.Enums;
using WSUI.Core.Interfaces;
using WSUI.Core.Core.Rules;
using WSUI.Core.Logger;
using WSUI.Core.Utils;

namespace WSUI.Core.Core.Search 
{
	public class BaseSearchRule<T> : ISearch, ISearchRule, IRuleQueryGenerator where T : class
    {
        #region [needs private]

	    private const string ConnectionString = "Provider=Search.CollatorDSO;Extended Properties=\"Application=Windows\"";
	    private Thread _ruleThread;
        private IQueryReader _reader;
	    private volatile bool _needStop = false;

        #endregion

        #region [needs protected]

	    protected volatile bool IsInterupt = false;
        protected IList<T> Result;
        protected bool NeedInterrup;
        protected DateTime LastDate;
        protected string Query;
        protected AutoResetEvent Event;
        protected int TopQueryResult = 100;
	    protected int CountFirstProcess = 35;
	    protected int CountSecondProcess = 7;
        protected int CountAdded = 0;
        protected int CountProcess = 0;


        // results
	    private TypeResult _typeResult;
	    private IList<IResultMessage> _listMessage;

        protected string QueryAnd = " AND \"{0}\"";

	    protected string RuleName;

        #endregion
        
        protected readonly object Lock = new object();

		protected BaseSearchRule()
		{   
            Event = new AutoResetEvent(false);
            Result = new List<T>();
		}

		public ISearchResult GetResults()
		{
            return new SearchResult(_typeResult, _listMessage.ToList(), (IEnumerable<ISearchObject>)Result.ToList());
		}

		/// <summary>
		/// @param ="criteris"
		/// </summary>
		/// <param name="criteria"></param>
		public void SetSearchCriteria(string criteria)
		{
		    Query = criteria;
		}

		public void Search()
		{
		    InitBeforeSearching();
            _ruleThread = new Thread(DoQuery) {Priority = ThreadPriority.Highest};
		    _ruleThread.Start();
		}

		public void Stop()
		{
		    _needStop = true;
		}

		public event Action<object> SearchStarted;

		public event Action<object> SearchFinished;

	    protected virtual void DoQuery()
	    {
	        try
	        {
	            IsSearching = true;
	            string query = QueryGenerator.Instance.GenerateQuery(typeof (T), Query, TopQueryResult, this, false);
	            if (string.IsNullOrEmpty(query))
                    throw new ArgumentNullException("Query is null or empty");
                WSSqlLogger.Instance.LogInfo("Query<{0}>: {1}",typeof(T).Name, query);
	            var connection = new OleDbConnection(ConnectionString);
                var cmd = new OleDbCommand(query, connection);
                cmd.CommandTimeout = 0;

                var watch = new Stopwatch();
                watch.Start();
                try
                {

                    connection.Open();

                    var watchOleDbCommand = new Stopwatch();
                    watchOleDbCommand.Start();
                    OleDbDataReader dataReader = cmd.ExecuteReader();
                    watchOleDbCommand.Stop();
                    WSSqlLogger.Instance.LogInfo("dataReader = cmd.ExecuteReader(); Elapsed: " + watchOleDbCommand.ElapsedMilliseconds.ToString());

                    while (dataReader.Read())
                    {
                        try
                        {
                            ReadData(dataReader);
                            if (IsInterupt)
                                break;
                        }
                        catch (Exception ex)
                        {
                            WSSqlLogger.Instance.LogError("{0}: {1}", "DoQuery _ main cycle", ex.Message);
                        }
                    }

                }
                catch (OleDbException oleDbException)
                {
                    WSSqlLogger.Instance.LogError("{0}: {1}", "DoQuery", oleDbException.Message);
                }
                // additional process
                ProcessResult();
                _typeResult = TypeResult.Ok;
	        }
	        catch (Exception ex)
	        {
                _typeResult = TypeResult.Error;
                _listMessage.Add(new ResultMessage(){Message = ex.Message});
                WSSqlLogger.Instance.LogError("Search: {0}",ex.Message);
	        }
	        finally
	        {
	            TopQueryResult = CountProcess = CountSecondProcess;
	            IsSearching = false;
	            Event.Set();
	        }
	        
	    }

	    protected virtual void CreateQuery()
	    {
	        IsInterupt = false;
	    }

	    /// 
	    /// <param name="reader"></param>
	    protected virtual void ReadData(IDataReader reader)
	    {
	        var result = Reader.ReadResult(reader) as T;
	        if (result == null)
	            return;
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
		    _needStop = false;
		    LastDate = GetCurrentDateTime();
            Result.Clear();
		}

	    public bool IsSearching { get; protected set; }
	    public int Priority { get; protected set; }

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

	    public string GenerateWherePart(IList<IRule> listCriteriaRules)
	    {
	        return OnGenerateWherePart(listCriteriaRules);
	    }

	    protected virtual string OnGenerateWherePart(IList<IRule> listCriterisRules)
	    {
	        return string.Empty;
	    }

        protected IQueryReader Reader
	    {
            get { return _reader ?? (_reader = QueryReader.CreateNewReader(typeof(T),FieldCash.Instance.GetFields(typeof(T),false))); }	        
	    }

	    protected DateTime GetCurrentDateTime()
	    {
	        return DateTime.Now.AddDays(1);
	    }

        protected string FormatDate(ref DateTime date)
        {
            return date.ToString("yyyy/MM/dd hh:mm:ss").Replace('.', '/');
        }

	    protected Tuple<string,List<string>> GetProcessingSearchCriteria(IList<IRule> listRuleCriteriasRules)
	    {
	        var tempCriteria = Query;
            var andClause = string.Empty;
            var listW = new List<string>();

            foreach (var rule in listRuleCriteriasRules.OrderBy(i => i.Priority))
            {
                listW.AddRange(rule.ApplyRule(Query));
                tempCriteria = rule.ClearCriteriaAccordingRule(tempCriteria);
            }

	        if (listW.Count > 1)
	        {
	            StringBuilder temp = new StringBuilder();
	            temp.Append(string.Format("'\"{0}*\"", listW[0]));
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
	        return new Tuple<string, List<string>>(andClause,listW.ToList());
	    }

	    protected virtual void InitBeforeSearching()
	    {
	        _typeResult = TypeResult.None;
	        _listMessage.Clear();
            Result.Clear();
	    }

	}//end BaseSearchRule

}//end namespace Search