using WSUI.Core.Interfaces;

namespace WSPreview.PreviewHandler.PreviewHandlerFramework
{
    public interface ITranslateMessage
    {
        void PassMessage(IWSAction action);
    }
}