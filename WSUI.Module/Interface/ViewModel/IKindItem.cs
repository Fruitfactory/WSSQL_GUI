using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WSUI.Core.Data;
using WSUI.Core.Interfaces;
using WSUI.Infrastructure.Services;
using WSUI.Module.Interface.Service;

namespace WSUI.Module.Interface.ViewModel
{
    public interface IKindItem
    {
        string Name
        {
            get;
            
        }

        string SearchString
        {
            get;
            set;
        }

        string Prefix
        {
            get;
            
        }

        int ID
        {
            get;
            
        }

        string UIName
        {
            get;
            
        }

        bool Toggle
        {
            get;
            set;
        }

        BaseSearchObject Current { get; set; }

        ObservableCollection<ISearchObject> DataSource
        {
            get;
        }

        IMainViewModel Parent { get; set; }

        List<string> FolderList { get; set; }
        string Folder { get; set; }
        bool Enabled { get; set; }
        ObservableCollection<IWSCommand> Commands { get; }
        
        ICommand SearchCommand { get; }
        ICommand OpenCommand { get; }
        ICommand KeyDownCommand { get; }
        ICommand ClearCriteriaCommand { get; }

        event EventHandler Start;

        event EventHandler<EventArgs<bool>> Complete;

        event EventHandler<EventArgs<bool>> Error;

        event EventHandler<EventArgs<BaseSearchObject>> CurrentItemChanged;

        event EventHandler Choose;

        void Init();
        void FilterData();

    }
}