﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using WSUI.Core.Data;
using WSUI.Core.Interfaces;

namespace WSUI.Module.Interface.ViewModel
{
    public interface IContactDetailsViewModel
    {
        object View { get; }

        ObservableCollection<AttachmentSearchObject> ItemsSource { get; }

        void SetDataObject(ISearchObject dataSearchObject);

        ISearchObject SelectedAttachement { get; set; }

    }
}