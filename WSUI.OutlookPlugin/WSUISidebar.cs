using System;
using System.Windows;
using WSUI.Control;
using WSUI.Core.Data;
using WSUI.Core.Enums;
using WSUI.Core.Helpers;
using WSUI.Infrastructure.Helpers.Extensions;
using WSUIOutlookPlugin.Interfaces;

namespace WSUIOutlookPlugin
{
    public partial class WSUISidebar : AddinExpress.OL.ADXOlForm, ISidebarForm
    {
        private IPluginBootStraper _wsuiBootStraper = null;
        private bool _isDebugMode = false;
        private bool _isSecondInstance = false;

        public WSUISidebar()
        {
            InitializeComponent();
            ADXAfterFormHide += OnAdxAfterFormHide;
        }

        private void OnAdxAfterFormHide(object sender, ADXAfterFormHideEventArgs e)
        {
            if (!_isSecondInstance)
                WSUIAddinModule.CurrentInstance.IsMainUIVisible = false;
            _isSecondInstance = false;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            SetBootStraper(WSUIAddinModule.CurrentInstance.BootStraper);
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
            var parent = el.GetParentProperty();
            if (el != null && parent == null)
            {
                wpfSidebarHost.Child = null;
                wpfSidebarHost.Child = el;
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
            else
            {
                _isSecondInstance = true;
                Hide();
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