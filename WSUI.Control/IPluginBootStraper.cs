using System.Windows;
using Microsoft.Practices.Unity;
using WSUI.Core.Interfaces;

namespace WSUI.Control
{
    public interface IPluginBootStraper
    {
        void Run();
        void PassAction(IWSAction action);
        DependencyObject View { get; }
        IUnityContainer Container { get; }
        bool IsPluginBusy { get; }
        bool IsMainUiActive { get; }
    }
}