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
    [PreviewHandler("WSSQL Office Preview Handler", ".doc;.docx;.xls", "{288E3293-B332-422C-BF5E-18BC8672C13A}")]
    [ProgId("C4F.DevKit.PreviewHandler.PreviewHandlers.OfficePreviewHandler")]
    [Guid("E289F0A5-EFFB-4B29-822B-CB1849D62426")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public sealed class OfficePreviewHandler : FileBasedPreviewHandler
    {
        private OfficePreviewHandlerControl _ctrl;

        protected override PreviewHandlerControl CreatePreviewHandlerControl()
        {
            _ctrl = new OfficePreviewHandlerControl();
            return _ctrl;
        }

        private sealed class OfficePreviewHandlerControl : FileBasedPreviewHandlerControl
        {
            private OfficeFilePreview _preview;
            public override void Load(FileInfo file)
            {
                _preview = new OfficeFilePreview();
                _preview.Dock = DockStyle.Fill;
                Controls.Add(_preview);
                _preview.LoadFile(file.FullName);
            }

            public override void Unload()
            {
                base.Unload();
                if (_preview == null)
                    return;
                Controls.Clear();
                _preview.Unload();
            }
        }

    }
}
