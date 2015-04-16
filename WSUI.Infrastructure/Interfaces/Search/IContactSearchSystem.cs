using System;
using System.Collections;
using System.Collections.Generic;
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

        void Initialize();

        void ResetPreviewSystem();

        void ResetMainSystem();

    }
}