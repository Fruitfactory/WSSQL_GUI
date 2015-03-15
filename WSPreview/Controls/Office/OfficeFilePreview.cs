using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WSPreview.PreviewHandler.PInvoke;
using WSPreview.PreviewHandler.PreviewHandlerFramework;
using WSUI.Core.Data;

namespace WSPreview.PreviewHandler.Controls.Office
{
    [KeyControl(ControlsKey.Office)]
    public partial class OfficeFilePreview : UserControl,IPreviewControl
    {
        private BaseOfficePreview _officePreviewGeneratorPreview;

        public OfficeFilePreview()
        {
            InitializeComponent();
        }

        public void LoadFile(string file)
        {
            _officePreviewGeneratorPreview = OfficePreviewFactory.CreatePreviewWindow(file);
            if (_officePreviewGeneratorPreview == null)
                throw new Exception("Office File Preview Error");
            _officePreviewGeneratorPreview.CreateApp();
            _officePreviewGeneratorPreview.LoadFile(file);
            if (!string.IsNullOrEmpty(_officePreviewGeneratorPreview.Filename))
            {
                webBrowserPreview.Url = new Uri(_officePreviewGeneratorPreview.Filename);
            }
        }

        public void LoadFile(Stream stream)
        {
        }

        public void LoadObject(BaseSearchObject obj)
        {
            
        }

        public void Clear()
        {
            Unload();
        }

        public void Unload()
        {
            if (_officePreviewGeneratorPreview == null)
                return;
            _officePreviewGeneratorPreview.UnloadApp();
        }

    }
}
