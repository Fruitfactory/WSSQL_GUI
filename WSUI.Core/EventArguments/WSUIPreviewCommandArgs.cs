using OF.Core.Enums;

namespace OF.Core.EventArguments
{
    public class OFPreviewCommandArgs : System.EventArgs
    {
        public OFPreviewCommandArgs(OFPreviewCommand cmd, object tag)
        {
            PreviewCommand = cmd;
            Tag = tag;
        }

        public OFPreviewCommand PreviewCommand { get; private set; }

        public object Tag { get; private set; }
    }
}