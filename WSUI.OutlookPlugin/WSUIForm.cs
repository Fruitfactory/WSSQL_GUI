using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using AddinExpress.OL;
using WSUI.Control;
  
namespace WSUIOutlookPlugin
{
    public partial class WSUIForm : AddinExpress.OL.ADXOlForm
    {

        private PluginBootStraper _wsuiBootStraper = null;

        public WSUIForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _wsuiBootStraper = new PluginBootStraper(wpfHost);
            _wsuiBootStraper.Run();
        }

    }
}
