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
        DependencyObject SidebarView { get; }
        IUnityContainer Container { get; }
        bool IsPluginBusy { get; }
    }
}