using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using AddinExpress.MSO;
using WSUI.Core.Logger;
using WSUIOutlookPlugin.About;
using WSUIOutlookPlugin.Core;

namespace WSUIOutlookPlugin.Managers
{
    public class WSUIAboutCommandManager : BaseCommandManager
    {
        #region [needs]

        private ADXRibbonButton _buttonHelp;
        private ADXRibbonButton _buttonAbout;
        //main toolbar
        private ADXRibbonButton _buttonMainHelp;
        private ADXRibbonButton _buttonMainAbout;

        #endregion

        public WSUIAboutCommandManager(ADXRibbonButton buttonHelp, ADXRibbonButton buttonAbout, ADXRibbonButton buttonMainHelp, ADXRibbonButton buttonMainAbout)
        {
            _buttonHelp = buttonHelp;
            _buttonAbout = buttonAbout;
            _buttonMainAbout = buttonMainAbout;
            _buttonMainHelp = buttonMainHelp;
            _buttonAbout.OnClick += ButtonAboutOnOnClick;
            _buttonHelp.OnClick += ButtonHelpOnOnClick;
            _buttonMainAbout.OnClick += ButtonAboutOnOnClick;
            _buttonMainHelp.OnClick += ButtonHelpOnOnClick;
        }

        private void ButtonHelpOnOnClick(object sender, IRibbonControl control, bool pressed)
        {
            try
            {
                string helpurl = Properties.Settings.Default.HelpUrl;
                if (string.IsNullOrEmpty(helpurl))
                {
                    WSSqlLogger.Instance.LogError("Run help: {0}", "Help url is empty");
                    return;
                }
                Process.Start(helpurl);
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError("Run help: {0}",ex.Message);
            }
        }

        private void ButtonAboutOnOnClick(object sender, IRibbonControl control, bool pressed)
        {
            WSUIAbout frmAbout = new WSUIAbout();
            frmAbout.ShowDialog();
        }
    }
}