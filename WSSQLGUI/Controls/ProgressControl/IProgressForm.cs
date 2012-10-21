using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WSSQLGUI.Controls.ProgressControl
{
    internal enum ProgressFormCommand
    {
        None,
        Settings,
        Activate,
        Progress
    }    

    internal interface IProgressForm
    {
        void CloseExt();
        void ProcessCommand(ProgressFormCommand cmd, object arg);

        FormWindowState State { get; set; }
        Point Location { get; set; }
        int Width { get; set; }
        int Height { get; set; }
    }
}
