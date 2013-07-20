using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WSPreview.PreviewHandler.PInvoke;

namespace WSPreview.PreviewHandler.Controls
{
    class PreviewBrowser : WebBrowser
    {

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WindowAPI.WM_COPY:
                case WindowAPI.WM_KEYDOWN:
                case WindowAPI.WM_CHAR:
                    System.Diagnostics.Debug.WriteLine("key down");
                    break;
            }
            base.WndProc(ref m);
        }

    }
}
