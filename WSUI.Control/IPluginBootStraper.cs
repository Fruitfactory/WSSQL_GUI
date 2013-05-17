using C4F.DevKit.PreviewHandler.Service;

namespace WSUI.Control
{
    public interface IPluginBootStraper
    {
        void Run();
        void PassAction(WSActionType actionType);
    }
}