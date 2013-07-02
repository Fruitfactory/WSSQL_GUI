using System;
using System.Diagnostics;
using System.Drawing;
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
    public partial class WSUIForm : AddinExpress.OL.ADXOlForm, ICleaneable
    {

        private IPluginBootStraper _wsuiBootStraper = null;

        public WSUIForm()
        {
            InitializeComponent();
            HookManager.KeyDown += HookManagerOnKeyDown;
            WSSqlLogger.Instance.LogInfo("WSUIForm [ctor]");
        }

        private void HookManagerOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs != null && (Control.ModifierKeys & Keys.Control) == Keys.Control && keyEventArgs.KeyCode == Keys.C && Visible)
            {
                _wsuiBootStraper.PassAction(WSActionType.Copy);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stopwatch watch = new Stopwatch();
            watch.Start();
            _wsuiBootStraper = new PluginBootStraper(wpfHost); 
            watch.Stop();
            WSSqlLogger.Instance.LogInfo(string.Format("OnLoad Main form (new PluginBootStraper(wpfHost)): {0}ms", watch.ElapsedMilliseconds));
            (watch = new Stopwatch()).Start();
            _wsuiBootStraper.Run();
            watch.Stop();
            WSSqlLogger.Instance.LogInfo(string.Format("OnLoad Main form (_wsuiBootStraper.Run()): {0}ms", watch.ElapsedMilliseconds));
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
        }
    }
}
