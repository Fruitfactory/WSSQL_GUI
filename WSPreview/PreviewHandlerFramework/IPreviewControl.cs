using System.IO;
using WSUI.Core.Data;

namespace WSPreview.PreviewHandler.PreviewHandlerFramework
{
    public interface IPreviewControl
    {
        void LoadFile(string filename);
        void LoadFile(Stream stream);
        void LoadObject(BaseSearchObject obj);
        void Clear();
    }
}