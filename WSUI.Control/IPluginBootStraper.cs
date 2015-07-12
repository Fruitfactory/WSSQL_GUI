using System;
using System.Windows;
using Microsoft.Practices.Unity;
using OF.Core.Enums;
using OF.Core.Interfaces;

namespace OF.Control
{
    public interface IPluginBootStraper
    {
        void Run();
        void PassAction(IWSAction action);
        DependencyObject View { get; }
        IUnityContainer Container { get; }
        bool IsPluginBusy { get; }
        bool IsMainUiActive { get; }

        bool IsMenuEnabled { get; }
        
    }
}