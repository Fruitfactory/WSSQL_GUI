using System.IO;
using WSPreview.PreviewHandler.PreviewHandlerFramework;
using System.ComponentModel.Design;

namespace WSPreview.Controls.BinControl
{
    [KeyControl(ControlsKey.Bin)]
    public class BinPreviewControl : ByteViewer, IPreviewControl
    {

        public void LoadFile(string filename)
        {
            SetFile(filename);
        }

        public void LoadFile(Stream stream)
        {
            
        }

        public void Clear()
        {
            Controls.Clear();
        }
    }
}