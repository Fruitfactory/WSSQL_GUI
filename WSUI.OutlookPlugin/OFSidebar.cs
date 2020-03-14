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
    public partial class OFSidebar :  ISidebarForm
    {
        private IPluginBootStraper _wsuiBootStraper = null;
        private bool _isSecondInstance = false;

        public OFSidebar()
        {
	        //ADXAfterFormHide += OnAdxAfterFormHide;
            OFLogger.Instance.LogDebug("Create OFSidebar");
        }

        private void OnAdxAfterFormHide(object sender, EventArgs e)
        {
            if (!_isSecondInstance)
                //OFAddinModule.CurrentInstance.IsMainUIVisible = false;
            _isSecondInstance = false;
        }

        //TODO
        //protected override void OnShown(EventArgs e)
        //{
        //    base.OnShown(e);

        //    SetBootStraper(OFAddinModule.CurrentInstance.BootStraper);
        //}

        //TODO:
        bool ISidebarForm.IsDisposed
        {
            get { return true; }
        }

        protected void OnLoad(EventArgs e)
        {
            //DoubleBuffered = true;
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
                    //Hide();
                    //OFAddinModule.CurrentInstance.IsMainUIVisible = false;
                }
                else
                {
                    //OFAddinModule.CurrentInstance.IsMainUIVisible = true;
                }
            }
            else
            {
                _isSecondInstance = true;
                //Hide();
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

        public void Hide ( )
        {
            throw new NotImplementedException();
        }

        public void Show ( )
        {
            throw new NotImplementedException();
        }
    }
}