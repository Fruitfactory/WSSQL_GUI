using System;
using System.Windows;
using WSUI.Control;
using WSUIOutlookPlugin.Interfaces;

namespace WSUIOutlookPlugin
{
    public partial class WSUISidebar : AddinExpress.OL.ADXOlForm, ISidebarForm
    {
        private IPluginBootStraper _wsuiBootStraper = null;

        public WSUISidebar()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SetBootStraper(WSUIAddinModule.CurrentInstance.BootStraper);
        }

        public void SetBootStraper(IPluginBootStraper bootStraper)
        {
            _wsuiBootStraper = bootStraper;
            UIElement el = _wsuiBootStraper.SidebarView as UIElement;
            if (el != null)
            {
                wpfSidebarHost.Child = el;
            }
        }
    }
}