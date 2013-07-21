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

namespace WSPreview.PreviewHandler.Controls.Office
{
    [KeyControl(ControlsKey.Office)]
    public partial class OfficeFilePreview : UserControl,IPreviewControl
    {
        private BaseOfficeWindow _window;

        public OfficeFilePreview()
        {
            InitializeComponent();
        }

        public void LoadFile(string file)
        {
            _window = OfficeWindowFactory.CreatePreviewWindow(file);
            if (_window == null)
                throw new Exception("Office File Preview Error");
            _window.CreateApp();
            _window.SetParentControl(this);
            _window.LoadFile(file);
        }

        public void LoadFile(Stream stream)
        {
        }

        public void Clear()
        {
            Unload();
        }

        public void Unload()
        {
            if (_window == null)
                return;
            _window.RestoreOfficeSettings();
            _window.UnloadApp();
        }


        private void OfficeFilePreview_Resize(object sender, EventArgs e)
        {
            if (_window == null)
                return;
            _window.ResizeWindow();
        }

    }
}
