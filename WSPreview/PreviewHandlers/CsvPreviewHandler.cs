// Stephen Toub
// Coded and published in January 2007 issue of MSDN Magazine 
// http://msdn.microsoft.com/msdnmag/issues/07/01/PreviewHandlers/default.aspx

using System;
using System.IO;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Globalization;
using C4F.DevKit.PreviewHandler.PreviewHandlerFramework;
using C4F.DevKit.PreviewHandler.Service.CsvHelper;
using C4F.DevKit.PreviewHandler.Controls.CsvControl;

namespace C4F.DevKit.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("MSDN Magazine CSV Preview Handler", ".csv", "{5F1DA711-99CA-4C7B-B314-90DD9D23E525}")]
    [ProgId("C4F.DevKit.PreviewHandler.PreviewHandlers.CsvPreviewHandler")]
    [Guid("9834EBE8-DA5E-465E-9C51-3B5E4F13C015")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public sealed class CsvPreviewHandler : StreamBasedPreviewHandler
    {
        protected override PreviewHandlerControl CreatePreviewHandlerControl()
        {
            return new CsvPreviewHandlerControl();
        }

        private sealed class CsvPreviewHandlerControl : StreamBasedPreviewHandlerControl
        {
            public override void Load(Stream stream)
            {
                CsvPreview preview = new CsvPreview();
                preview.LoadFile(stream);
                preview.Dock = DockStyle.Fill;
                Controls.Add(preview);
            }

        }
    }
}