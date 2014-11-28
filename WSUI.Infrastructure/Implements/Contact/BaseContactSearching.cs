using System;
using System.Collections.Generic;
using WSUI.Core.Interfaces;
using WSUI.Infrastructure.Interfaces.Search;

namespace WSUI.Infrastructure.Implements.Contact
{
    public abstract class BaseContactSearching : IContactSearchSystem
    {
        private ISearchSystem _previewSystem;
        private ISearchSystem _mainSystem;

        protected BaseContactSearching()
        {
        }

        private void MainSystemOnSearchFinished(object o)
        {
            var temp = MainSearchingFinished;
            if (temp != null)
            {
                temp(this, EventArgs.Empty);
            }
        }

        private void PreviewSystemOnSearchFinished(object o)
        {
            var temp = PreviewSearchingFinished;
            if (temp != null)
            {
                temp(this, EventArgs.Empty);
            }
        }

        public void Initialize()
        {
            _previewSystem = GetPreviewSystem();
            _mainSystem = GetMainSystem();
            _previewSystem.SearchFinished += PreviewSystemOnSearchFinished;
            _mainSystem.SearchFinished += MainSystemOnSearchFinished;
        }

        public void ResetPreviewSystem()
        {
            _previewSystem.Reset();
        }

        public void ResetMainSystem()
        {
            _mainSystem.Reset();
        }

        protected abstract ISearchSystem GetPreviewSystem();
        protected abstract ISearchSystem GetMainSystem();

        public event EventHandler PreviewSearchingFinished;
        public event EventHandler MainSearchingFinished;
        public IList<ISystemSearchResult> GetPreviewResult()
        {
            return _previewSystem.GetResult();
        }

        public IList<ISystemSearchResult> GetMainResult()
        {
            return _mainSystem.GetResult();
        }

        public void SetSearchCriteria(string criteria)
        {
            _previewSystem.SetSearchCriteria(criteria);
            _mainSystem.SetSearchCriteria(criteria);
        }

        public void StartSearch()
        {
            _previewSystem.Search();
            _mainSystem.Search();
        }

        public void StartPreviewSearch()
        {
            _previewSystem.Search();
        }

        public void StartMainSearch()
        {
            _mainSystem.Search();
        }

        
    }
}