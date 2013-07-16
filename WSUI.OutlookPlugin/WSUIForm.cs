using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using AddinExpress.OL;
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
            Stopwatch watch = new Stopwatch();
            watch.Start();
            //_wsuiBootStraper = new PluginBootStraper(); 
            watch.Stop();
            WSSqlLogger.Instance.LogInfo(string.Format("OnLoad Main form (new PluginBootStraper(wpfHost)): {0}ms", watch.ElapsedMilliseconds));
            (watch = new Stopwatch()).Start();
            SetBootStraper(WSUIAddinModule.CurrentInstance.BootStraper);
            //_wsuiBootStraper.Run();
            watch.Stop();
            WSSqlLogger.Instance.LogInfo(string.Format("OnLoad Main form (_wsuiBootStraper.Run()): {0}ms", watch.ElapsedMilliseconds));
        }

        public void PassActionType(WSActionType actionType)
        {
            if(_isDebugMode)
                return;
            
            switch (actionType)
            {
                case WSActionType.Copy:
                    //_wsuiBootStraper.PassAction(actionType);
                    break;
                case WSActionType.Paste:
                    break;
            }
        }

        public void SetBootStraper(IPluginBootStraper bootStraper)
        {
            
            _wsuiBootStraper = bootStraper;
            wpfHost.Child = _wsuiBootStraper.View as UIElement;
        }
    }
}
