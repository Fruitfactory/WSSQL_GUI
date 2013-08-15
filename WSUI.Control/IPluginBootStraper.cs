using System.Windows;
using WSUI.Core.Interfaces;

namespace WSUI.Control
{
    public interface IPluginBootStraper
    {
        void Run();
        void PassAction(IWSAction action);
        DependencyObject View { get; }
    }
}