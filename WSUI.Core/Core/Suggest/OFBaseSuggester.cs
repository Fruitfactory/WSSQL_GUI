using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Practices.Unity;
using Nest;
using OF.Core.Interfaces;
using OF.Core.Logger;

namespace OF.Core.Core.Suggest
{
    public abstract class OFBaseSuggester<T> where T:class, IElasticSearchObject, new()
    {
        #region [needs]

        private Thread _thread;
        private volatile bool _isSearching = false;
        private IElasticSearchClient<T> _elasticSearchClient;
        private readonly object _lock = new object();
        private string _searchString;
        private IEnumerable<string> _result;
        private CancellationToken? _cancelToken;

        private readonly AutoResetEvent _event = new AutoResetEvent(false);

        #endregion


        protected OFBaseSuggester(IUnityContainer unityContainer)
        {
            _elasticSearchClient = unityContainer.Resolve<IElasticSearchClient<T>>();
        }


        public IEnumerable<string> GetResult()
        {
            return null;
        }

        public AutoResetEvent Event
        {
            get { return _event;}
        }

        public void Search(string searchString, CancellationToken cancelToken)
        {
            if (_isSearching)
            {
                OFLogger.Instance.LogInfo("Suggesting is running...");
                return;
            }
            if (string.IsNullOrEmpty(searchString))
            {
                throw new ArgumentNullException(searchString);
            }
            
            _thread = new Thread(DoSearch) {Priority = ThreadPriority.Highest,Name = string.Format("{0}_{1}",GetType().Name,typeof(T).Name)};
            _thread.Start();
        }

        public void Reset()
        {
            _isSearching = false;
            _cancelToken = null;
            _event.Reset();
            _searchString = null;
            _result = null;
        }
        
        private void DoSearch()
        {
            _isSearching = true;
            try
            {
                var response = _elasticSearchClient.ElasticClient.Search<T>(GetSearchDescriptor);
                _cancelToken.Value.ThrowIfCancellationRequested();
                if (response != null && response.ApiCall.Success)
                {
                    // TODO: investigate suggesting in ES - NEST 5.3
                    //_result = ProcessResponse(response.);
                }
            }
            catch (OperationCanceledException c)
            {
                OFLogger.Instance.LogError(c.ToString());
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
            finally
            {
                _isSearching = false;
                Event.Set();
            }
        }

        protected abstract SearchDescriptor<T> GetSearchDescriptor(SearchDescriptor<T> arg);

        // TODO: investigate suggesting in ES - NEST 5.3
        //private IEnumerable<string> ProcessResponse(IDictionary<string,Nest.Suggest[]> suggest)
        //{
        //    if (suggest == null)
        //    {
        //        return null;
        //    }
        //    var result = new List<string>();
        //    result.AddRange(suggest.SelectMany(s => s.Value).SelectMany(s => s.Options).Select(o => o.Text));
        //    return result;
        //}

    }
}