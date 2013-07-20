using System.Windows.Forms;
using WSPreview.PreviewHandler.Service;

namespace WSPreview.PreviewHandler.PreviewHandlerFramework
{
    public interface ITranslateMessage
    {
        void PassMessage(WSActionType action);
    }
}