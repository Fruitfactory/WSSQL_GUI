using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using C4F.DevKit.PreviewHandler.Service;
using C4F.DevKit.PreviewHandler.Service.Logger;
using WSUI.Control;
using WSUIOutlookPlugin.Interfaces;
using WSUIOutlookPlugin.Hooks;

namespace WSUIOutlookPlugin
{
    public partial class WSUIForm : AddinExpress.OL.ADXOlForm, IMainForm
    {

        private IPluginBootStraper _wsuiBootStraper = null;
        private bool _isDebugMode = false;

        public WSUIForm()
        {
            if (!Process.GetCurrentProcess().ProcessName.Equals("MSBuild", StringComparison.InvariantCultureIgnoreCase)) // recommendation from Add-In Express team
            {
                InitializeComponent();
                HookManager.KeyDown += HookManagerOnKeyDown;
                WSSqlLogger.Instance.LogInfo("WSUIForm [ctor]");
                ADXAfterFormHide += OnAdxAfterFormHide;
            }
            else
            {
                _isDebugMode = true;
                WSSqlLogger.Instance.LogInfo("Debug Mode ON...");
            }

        }

        private void HookManagerOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs != null && (Control.ModifierKeys & Keys.Control) == Keys.Control && keyEventArgs.KeyCode == Keys.C && Visible && _wsuiBootStraper != null)
            {
                _wsuiBootStraper.PassAction(WSActionType.Copy);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            if(_isDebugMode)
                return;
            base.OnLoad(e);
            SetBootStraper(WSUIAddinModule.CurrentInstance.BootStraper);
        }

        public void SetBootStraper(IPluginBootStraper bootStraper)
        {
            _wsuiBootStraper = bootStraper;
            UIElement el = _wsuiBootStraper.View as UIElement;
            if (el != null)
            {
                wpfHost.Child = _wsuiBootStraper.View as UIElement;
            }    
        }

        private void OnAdxAfterFormHide(object sender, ADXAfterFormHideEventArgs adxAfterFormHideEventArgs)
        {
            wpfHost.Child = null;
        }

    }
}
