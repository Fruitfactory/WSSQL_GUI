using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using AddinExpress.OL;
using C4F.DevKit.PreviewHandler.Service;
using WSUI.Control;
using Kennedy;
using Kennedy.ManagedHooks;
using WSUIOutlookPlugin.Interfaces;

namespace WSUIOutlookPlugin
{
    public partial class WSUIForm : AddinExpress.OL.ADXOlForm, ICleaneable
    {

        private IPluginBootStraper _wsuiBootStraper = null;
        private KeyboardTracking _keyboardTracking = null;

        public WSUIForm()
        {
            InitializeComponent();
            
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _keyboardTracking = new KeyboardTracking();
            _keyboardTracking.KeyDown += KeyboardTrackingOnKeyDown;
            _keyboardTracking.InstallHook();
        }

        private void KeyboardTrackingOnKeyDown(Keys key)
        {
            if (!ReferenceEquals(_keyboardTracking, null) &&
               _keyboardTracking.ControlPressed &&
               key == Keys.C &&
               Visible)
            {
                _wsuiBootStraper.PassAction(WSActionType.Copy);
            }
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
                    //_wsuiBootStraper.PassAction(actionType);
                    break;
                case WSActionType.Paste:
                    break;
            }
        }

        public void Clean()
        {
            if (!ReferenceEquals(_keyboardTracking, null))
            {
                _keyboardTracking.KeyDown -= KeyboardTrackingOnKeyDown;
                _keyboardTracking.UninstallHook();
                _keyboardTracking.Dispose();
                _keyboardTracking = null;
            }
        }
    }
}
