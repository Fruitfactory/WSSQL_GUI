using System.Windows;
using WSPreview.PreviewHandler.Service;

namespace WSUI.Control
{
    public interface IPluginBootStraper
    {
        void Run();
        void PassAction(WSActionType actionType);
        DependencyObject View { get; }
    }
}