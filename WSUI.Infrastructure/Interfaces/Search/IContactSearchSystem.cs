using System;
using System.Collections;
using System.Collections.Generic;
using WSUI.Core.Interfaces;

namespace WSUI.Infrastructure.Interfaces.Search
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