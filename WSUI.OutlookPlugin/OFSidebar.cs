using System;
using System.Windows;
using OF.Control;
using OF.Core.Data;
using OF.Core.Enums;
using OF.Core.Helpers;
using OF.Core.Logger;
using OF.Core.Extensions;
using OFOutlookPlugin.Interfaces;

namespace OFOutlookPlugin
{
    public partial class OFSidebar : AddinExpress.OL.ADXOlForm, ISidebarForm
    {
        private IPluginBootStraper _wsuiBootStraper = null;
        private bool _isDebugMode = false;
        private bool _isSecondInstance = false;

        public OFSidebar()
        {
            InitializeComponent();
            ADXAfterFormHide += OnAdxAfterFormHide;
            OFLogger.Instance.LogDebug("Create OFSidebar");
        }

        private void OnAdxAfterFormHide(object sender, ADXAfterFormHideEventArgs e)
        {
            if (!_isSecondInstance)
                OFAddinModule.CurrentInstance.IsMainUIVisible = false;
            _isSecondInstance = false;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            SetBootStraper(OFAddinModule.CurrentInstance.BootStraper);
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
            if (bootStraper == null)
            {
                OFLogger.Instance.LogDebug("Bootstraper eqaul 'NULL'.");
                return;
            }
            _wsuiBootStraper = bootStraper;
            UIElement el = _wsuiBootStraper.View as UIElement;
            var parent = el.GetParentProperty();
            if (el != null && parent == null)
            {
                wpfSidebarHost.Child = null;
                wpfSidebarHost.Child = el;
                if (!OFRegistryHelper.Instance.GetIsPluginUiVisible())
                {
                    Hide();
                    OFAddinModule.CurrentInstance.IsMainUIVisible = false;
                }
                else
                {
                    OFAddinModule.CurrentInstance.IsMainUIVisible = true;
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

        public void SendAction(OFActionType actionType)
        {
            _wsuiBootStraper.PassAction(new OFAction(actionType, null));
        }

    }
}