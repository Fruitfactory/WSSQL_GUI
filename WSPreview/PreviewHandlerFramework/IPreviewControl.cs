using System.IO;
using OF.Core.Data;

namespace OFPreview.PreviewHandler.PreviewHandlerFramework
{
    public interface IPreviewControl
    {
        void LoadFile(string filename);
        void LoadFile(Stream stream);
        void LoadObject(OFBaseSearchObject obj);
        void Clear();
    }
}