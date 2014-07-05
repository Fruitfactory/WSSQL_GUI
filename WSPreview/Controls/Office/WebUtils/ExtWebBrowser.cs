using System;
using System.Security.Permissions;
using System.Windows.Forms;

namespace WSPreview.PreviewHandler.Controls.Office.WebUtils
{
    public partial class ExtWebBrowser : WebBrowser
    {
        private AxHost.ConnectionPointCookie _cookie;
        private ExtWebBrowserEventHelper _eventHelper;
        private bool _renavigating;

        public EventHandler<WebBrowserNavigatingEventArgs> BeforeNavigate;

        public ExtWebBrowser()
        {
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void CreateSink()
        {
            base.CreateSink();
            _eventHelper = new ExtWebBrowserEventHelper(this);
            _cookie = new AxHost.ConnectionPointCookie(ActiveXInstance, _eventHelper, typeof(DWebBrowserEvents2));
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void DetachSink()
        {
            if (_cookie != null)
            {
                _cookie.Disconnect();
                _cookie = null;
            }
            base.DetachSink();
        }

        private void OnBeforeNavigate2(object pDisp,
                           ref object url,
                           ref object flags,
                           ref object targetFrameName,
                           ref object postData,
                           ref object headers,
                           ref bool cancel)
        {
            if (!_renavigating)
            {
                EventHandler<WebBrowserNavigatingEventArgs> temp = this.BeforeNavigate;
                if (temp != null)
                {
                    WebBrowserNavigatingEventArgs args = new WebBrowserNavigatingEventArgs(new Uri((string)url), (string)targetFrameName);
                    temp(this, args);
                    cancel = args.Cancel;
                }
            }
            else
            {
                _renavigating = false;
            }
        }
    }
}