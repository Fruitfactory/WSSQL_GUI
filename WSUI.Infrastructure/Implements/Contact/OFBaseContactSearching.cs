﻿using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using OF.Core.Interfaces;
using OF.Infrastructure.Interfaces.Search;

namespace OF.Infrastructure.Implements.Contact
{
    public abstract class OFBaseContactSearching : IContactSearchSystem
    {
        protected readonly object LockObject = null;

        private ISearchSystem _previewSystem;
        private ISearchSystem _mainSystem;

        protected OFBaseContactSearching( object Lock )
        {
            LockObject = Lock;
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

        public void Initialize(IUnityContainer container)
        {
            _previewSystem = GetPreviewSystem(container);
            _mainSystem = GetMainSystem(container);
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

        protected abstract ISearchSystem GetPreviewSystem(IUnityContainer container);
        protected abstract ISearchSystem GetMainSystem(IUnityContainer container);

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