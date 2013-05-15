using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Globalization;
using C4F.DevKit.PreviewHandler;
using C4F.DevKit.PreviewHandler.PreviewHandlerFramework;
using C4F.DevKit.PreviewHandler.Controls.Office;

namespace C4F.DevKit.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("WSSQL Office Preview Handler", ".msg", "{CE4CB591-6E33-4CA1-9E0C-BD6F774AFEB1}")]
    [ProgId("C4F.DevKit.PreviewHandler.PreviewHandlers.OutlookPreviewHandler")]
    [Guid("326A2452-981E-403B-9921-911011E677E6")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public sealed class OutlookPreviewHandler : FileBasedPreviewHandler,ISearchWordHighlight,ITranslateMessage
    {

        protected override PreviewHandlerControl CreatePreviewHandlerControl()
        {
            return new OutlookPreviewHandlerControl(this);
        }

        public sealed class OutlookPreviewHandlerControl : FileBasedPreviewHandlerControl,ITranslateMessage
        {
            private OutlookFilePreview _preview;
            private OutlookPreviewHandler _parent;
            public OutlookPreviewHandlerControl(OutlookPreviewHandler parent)
            {
                _parent = parent;
            }


            public override void Load(FileInfo file)
            {
                _preview = new OutlookFilePreview();
                _preview.Dock = DockStyle.Fill;
                Controls.Add(_preview);
                _preview.HitString = _parent.HitString;
                _preview.LoadFile(file.FullName);
            }

            public override void Unload()
            {
                Controls.Clear();
                _preview.Unload();
                base.Unload();
            }

            public void PassMessage(Message m)
            {
                var translateMessage = _preview as ITranslateMessage;
                if (translateMessage != null)
                {
                    translateMessage.PassMessage(m);
                }
            }
        }

        public string HitString
        {
            get; set;
        }

        public void PassMessage(Message m)
        {
            if (_previewControl is ITranslateMessage)
            {
                ((ITranslateMessage)_previewControl).PassMessage(m);
            }
        }
    }
}
