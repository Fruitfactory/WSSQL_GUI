using System;
using System.Windows;
using AddinExpress.OL;
using WSUI.Control;
using WSUI.Core.Helpers;
using WSUIOutlookPlugin.Interfaces;

namespace WSUIOutlookPlugin
{
    public partial class WSUISidebar : AddinExpress.OL.ADXOlForm, ISidebarForm
    {
        private IPluginBootStraper _wsuiBootStraper = null;

        public WSUISidebar()
        {
            InitializeComponent();
            ADXAfterFormHide += OnAdxAfterFormHide;
        }

        private void OnAdxAfterFormHide(object sender, ADXAfterFormHideEventArgs e)
        {
            WSUIAddinModule.CurrentInstance.IsMainUIVisible = false;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (!RegistryHelper.Instance.GetIsPluginUiVisible())
            {
                Hide();
                WSUIAddinModule.CurrentInstance.IsMainUIVisible = false;
            }
            else
            {
                WSUIAddinModule.CurrentInstance.IsMainUIVisible = true;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SetBootStraper(WSUIAddinModule.CurrentInstance.BootStraper);
            RaiseOnLoaded();
            DoubleBuffered = true;
        }

        public void SetBootStraper(IPluginBootStraper bootStraper)
        {
            _wsuiBootStraper = bootStraper;
            UIElement el = _wsuiBootStraper.View as UIElement;
            if (el != null)
            {
                wpfSidebarHost.Child = el;
            }
        }

        public event EventHandler OnLoaded;
        public void Minimize()
        {
            throw new NotImplementedException();
        }

        private void RaiseOnLoaded()
        {
            var temp = OnLoaded;
            if (temp != null)
            {
                temp(this, EventArgs.Empty);
            }
        }
    }
}