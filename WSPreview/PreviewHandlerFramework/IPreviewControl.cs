using System.IO;

namespace WSPreview.PreviewHandler.PreviewHandlerFramework
{
    public interface IPreviewControl
    {
        void LoadFile(string filename);
        void LoadFile(Stream stream);
    }
}