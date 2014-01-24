using System;
using System.Collections.Generic;
using System.Windows.Input;
using WSUI.Core.Data;
using WSUI.Core.Enums;
using WSUI.Core.Interfaces;

namespace WSUI.Module.Interface
{
    public interface IMainViewModel
    {
        event EventHandler Start;
        event EventHandler Complete;
        List<BaseSearchObject> MainDataSource { get; }
        void Clear();
        void SelectKind(string name);
        void PassAction(IWSAction action);
        HostType Host { get;  }
        void ForceClosePreview();
        ActivationState ActivateStatus { get; }
        string TextStatus { get; }
        ICommand ActivateCommand { get; }
        ICommand DeactivateCommand { get; }
        ICommand TryAgainCommand { get; }
        bool IsBusy { get; }
    }
}
