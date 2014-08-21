﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Transitionals;
using WSUI.Core.Data;
using WSUI.Core.Enums;
using WSUI.Core.Interfaces;
using WSUI.Module.Service;

namespace WSUI.Module.Interface.ViewModel
{
    public interface IMainViewModel
    {
        event EventHandler Start;
        event EventHandler Complete;
        event EventHandler<SlideDirectionEventArgs> Slide;

        List<BaseSearchObject> MainDataSource { get; }
        void Clear();
        void SelectKind(string name);
        void PassAction(IWSAction action);
        HostType Host { get;  }
        void ForceClosePreview();
        ActivationState ActivateStatus { get; }
        ICommand BuyCommand { get; }
        ICommand BackCommand { get; }
        ICommand ActivateCommand { get; }
        bool IsBusy { get; }
        bool IsMainUiActive { get; }
        Visibility VisibleTrialLabel { get; }

        Visibility DataVisibility { get; }
        Visibility PreviewVisibility { get; }

        Transition CurrenTransition { get; }

        void ShowOutlookFolder(string folder);

    }
}