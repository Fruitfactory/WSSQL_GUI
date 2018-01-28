using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Transitionals;
using OF.Core.Data;
using OF.Core.Enums;
using OF.Core.Interfaces;
using OF.Module.Interface.Service;
using OF.Module.Service;

namespace OF.Module.Interface.ViewModel
{
    public interface IMainViewModel
    {
        event EventHandler Start;
        event EventHandler Complete;
        event EventHandler<OFSlideDirectionEventArgs> Slide;

        List<OFBaseSearchObject> MainDataSource { get; }
        void Clear();
        void SelectKind(string name);
        void PassAction(IWSAction action);
        OFHostType Host { get;  }
        void ForceClosePreview();
        OFActivationState ActivateStatus { get; }
        ICommand BuyCommand { get; }
        ICommand BackCommand { get; }
        ICommand ActivateCommand { get; }
        bool IsBusy { get; }
        bool IsMainUiActive { get; }
        Visibility VisibleTrialLabel { get; }

        Visibility DataVisibility { get; }
        Visibility BackButtonVisibility { get; }

        Transition CurrenTransition { get; set; }

        void ShowOutlookFolder(string folder);

        void ShowContactPreview(object tag, bool useTransaction);

        void ShowAdvancedSearch(object tag);

        OFBaseSearchObject Current { get; }

        OFBaseSearchObject CurrentTracked { get; }

        IEnumerable<MenuItem> EmailsMenuItems { get; }

        IEnumerable<MenuItem> FileMenuItems { get; }

        bool IsPreviewVisible { get; }

        bool IsMenuEnabled { get; }
        bool IsActivated { get; }
    }
}
