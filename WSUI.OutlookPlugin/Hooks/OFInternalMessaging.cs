using System;
using System.Windows.Forms;
using OF.Core.Logger;
using OF.Core.Win32;

namespace OFOutlookPlugin.Hooks
{
    public class OFInternalMessaging : NativeWindow, IDisposable
    {
        private bool _disposed;
        private ThisAddIn _outlookAddIn;


        public OFInternalMessaging(ThisAddIn outlookAddIn, string caption)
        {
            _outlookAddIn = outlookAddIn;
            CreateHandle(new CreateParams()
            {
                Caption = caption
            });
        }

        ~OFInternalMessaging() => Dispose(false);

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this._disposed && !this.Handle.Equals((object)IntPtr.Zero))
                this.DestroyHandle();
            this._disposed = true;
        }


        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WindowsFunction.WM_KEYDOWN:
                    OFLogger.Instance.LogDebug("WM_KEYDOWN");
                    break;
                case WindowsFunction.WM_LBUTTONDOWN:
                    OFLogger.Instance.LogDebug("WM_LBUTTONDOWN");
                    break;

            }
            base.WndProc(ref m);
        }
    }
}