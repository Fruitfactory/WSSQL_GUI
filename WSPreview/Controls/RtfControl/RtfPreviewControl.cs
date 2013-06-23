using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using C4F.DevKit.PreviewHandler.PreviewHandlerFramework;

namespace C4F.DevKit.PreviewHandler.Controls.RtfControl
{
    public partial class RtfPreviewControl : UserControl, IPreviewControl
    {
        public RtfPreviewControl()
        {
            InitializeComponent();
        }

        #region Implementation of IPreviewControl

        public void LoadFile(string filename)
        {
            if(string.IsNullOrEmpty(filename))
                return;
            try
            {
                rtfPreview.LoadFile(filename);
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public void LoadFile(Stream stream)
        {
        }

        #endregion
    }
}
