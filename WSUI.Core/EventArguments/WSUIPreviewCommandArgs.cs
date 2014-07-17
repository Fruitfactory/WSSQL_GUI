using WSUI.Core.Enums;

namespace WSUI.Core.EventArguments
{
    public class WSUIPreviewCommandArgs : System.EventArgs
    {
        public WSUIPreviewCommandArgs(WSPreviewCommand cmd, object tag)
        {
            PreviewCommand = cmd;
            Tag = tag;
        }

        public WSPreviewCommand PreviewCommand { get; private set; }

        public object Tag { get; private set; }
    }
}