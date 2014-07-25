using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using AddinExpress.OL;
using WSUI.Control;
using WSUI.Core.Data;
using WSUI.Core.Enums;
using WSUI.Core.Helpers;
using WSUI.Core.Logger;
using WSUIOutlookPlugin.Hooks;
using WSUIOutlookPlugin.Interfaces;

namespace WSUIOutlookPlugin
{
    public partial class WSUISidebar : AddinExpress.OL.ADXOlForm, ISidebarForm
    {
        private IPluginBootStraper _wsuiBootStraper = null;
        private bool _isDebugMode = false;
        
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
            SetBootStraper(WSUIAddinModule.CurrentInstance.BootStraper);
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

        bool ISidebarForm.IsDisposed
        {
            get { return this.IsDisposed; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            DoubleBuffered = true;
        }

        public void SetBootStraper(IPluginBootStraper bootStraper)
        {
            _wsuiBootStraper = bootStraper;
            UIElement el = _wsuiBootStraper.View as UIElement;
            if (el != null)
            {
                wpfSidebarHost.Child = null;
                wpfSidebarHost.Child = el;
            }
        }

        public void Minimize()
        {
            throw new NotImplementedException();
        }

        public void SendAction(WSActionType actionType)
        {
            _wsuiBootStraper.PassAction(new WSAction(actionType, null));
        }
    }
}