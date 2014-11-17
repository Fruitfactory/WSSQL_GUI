﻿using System;
using System.Collections.ObjectModel;
using WSUI.Core.Interfaces;

namespace WSUI.Module.Interface.ViewModel
{
    public interface IContactKindDetailsViewModel<T> : IDisposable where T : ISearchObject
    {
        object View { get; }

        ObservableCollection<T> ItemSource { get; }

        ObservableCollection<T> PreviewSource { get; }
        string SearchString { get; set; }

        ISearchObject SelectedObject { get; set; }

        ISearchObject TrackedObject { get; set; }
        bool IsBusy { get; }
        bool IsMoreVisible { get; }
        double Height { get; }

        void Initialize();

        void SetSearchContext(string from, string to);
    }
}