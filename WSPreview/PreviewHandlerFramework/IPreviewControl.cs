using System.IO;

namespace C4F.DevKit.PreviewHandler.PreviewHandlerFramework
{
    public interface IPreviewControl
    {
        void LoadFile(string filename);
        void LoadFile(Stream stream);
    }
}