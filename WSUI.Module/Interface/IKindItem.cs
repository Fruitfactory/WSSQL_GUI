using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using WSUI.Infrastructure.Core;
using WSUI.Infrastructure.Services;

namespace WSUI.Module.Interface
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

        BaseSearchData Current { get; set; }

        ObservableCollection<BaseSearchData> DataSource
        {
            get;
        }

        List<string> FolderList { get; set; }
        string Folder { get; set; }
        bool Enabled { get; set; }
        ObservableCollection<IWSCommand> Commands { get; }
        
        ICommand SearchCommand { get; }
        ICommand OpenCommand { get; }
        ICommand KeyDownCommand { get; }

        event EventHandler Start;

        event EventHandler<EventArgs<bool>> Complete;

        event EventHandler<EventArgs<bool>> Error;

        event EventHandler<EventArgs<BaseSearchData>> CurrentItemChanged;

        event EventHandler Choose;

        void Init();
    }
}
