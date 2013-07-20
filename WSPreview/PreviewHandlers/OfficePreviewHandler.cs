using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Globalization;
using WSPreview.PreviewHandler;
using WSPreview.PreviewHandler.PreviewHandlerFramework;
using WSPreview.PreviewHandler.Controls.Office;

namespace WSPreview.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("WSSQL Office Preview Handler", ".doc;.docx;.xls;.xlsx", "{288E3293-B332-422C-BF5E-18BC8672C13A}")]
    [ProgId("WSPreview.PreviewHandler.PreviewHandlers.OfficePreviewHandler")]
    [Guid("E289F0A5-EFFB-4B29-822B-CB1849D62426")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public sealed class OfficePreviewHandler : FileBasedPreviewHandler
    {
        protected override PreviewHandlerControl CreatePreviewHandlerControl()
        {
             return new OfficePreviewHandlerControl();
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
                if (_preview == null)
                    return;
                _preview.Unload();
                Controls.Clear();
                base.Unload();
            }
        }

    }
}
