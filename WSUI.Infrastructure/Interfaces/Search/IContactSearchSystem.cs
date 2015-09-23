using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using OF.Core.Interfaces;

namespace OF.Infrastructure.Interfaces.Search
{
    public interface IContactSearchSystem
    {
        event EventHandler PreviewSearchingFinished;
        event EventHandler MainSearchingFinished;

        IList<ISystemSearchResult> GetPreviewResult();
        IList<ISystemSearchResult> GetMainResult();

        void SetSearchCriteria(string criteria);

        void StartSearch();

        void StartPreviewSearch();

        void StartMainSearch();

        void Initialize(IUnityContainer container);

        void ResetPreviewSystem();

        void ResetMainSystem();

    }
}