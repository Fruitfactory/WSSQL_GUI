using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using AddinExpress.OL;
using C4F.DevKit.PreviewHandler.Service;
using WSUI.Control;
  
namespace WSUIOutlookPlugin
{
    public partial class WSUIForm : AddinExpress.OL.ADXOlForm
    {

        private IPluginBootStraper _wsuiBootStraper = null;

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

        public void PassActionType(WSActionType actionType)
        {
            switch (actionType)
            {
                case WSActionType.Copy:
                    _wsuiBootStraper.PassAction(actionType);
                    break;
                case WSActionType.Paste:
                    break;
            }
        }

    }
}
