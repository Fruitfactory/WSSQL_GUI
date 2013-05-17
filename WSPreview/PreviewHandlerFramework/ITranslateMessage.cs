using System.Windows.Forms;
using C4F.DevKit.PreviewHandler.Service;

namespace C4F.DevKit.PreviewHandler.PreviewHandlerFramework
{
    public interface ITranslateMessage
    {
        void PassMessage(WSActionType action);
    }
}