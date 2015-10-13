using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using AddinExpress.MSO;
using Microsoft.Practices.Prism.Events;
using OF.Core.Data;
using OF.Core.Enums;
using OF.Core.Events;
using OF.Core.Logger;
using OFOutlookPlugin.About;
using OFOutlookPlugin.Core;

namespace OFOutlookPlugin.Managers
{
    public class OFAboutCommandManager : BaseCommandManager
    {
        #region [needs]

        private ADXRibbonButton _buttonHelp;
        private ADXRibbonButton _buttonAbout;
        private ADXRibbonButton _buttonSettings;
        //main toolbar
        private ADXRibbonButton _buttonMainHelp;
        private ADXRibbonButton _buttonMainAbout;
        private ADXRibbonButton _buttonMainSettings;

        #endregion

        public OFAboutCommandManager(ADXRibbonButton buttonHelp, ADXRibbonButton buttonAbout, ADXRibbonButton buttonSettings, ADXRibbonButton buttonMainHelp, ADXRibbonButton buttonMainAbout,ADXRibbonButton buttonMainSettings)
        {
            _buttonHelp = buttonHelp;
            _buttonAbout = buttonAbout;
            _buttonSettings = buttonSettings;
            _buttonMainAbout = buttonMainAbout;
            _buttonMainHelp = buttonMainHelp;
            _buttonMainSettings = buttonMainSettings;
            _buttonAbout.OnClick += ButtonAboutOnOnClick;
            _buttonHelp.OnClick += ButtonHelpOnOnClick;
            _buttonMainAbout.OnClick += ButtonAboutOnOnClick;
            _buttonMainHelp.OnClick += ButtonHelpOnOnClick;
            _buttonSettings.OnClick += ButtonSettingsOnOnClick;
            _buttonMainSettings.OnClick += ButtonSettingsOnOnClick;
            MenuEnabling(OFAddinModule.CurrentInstance.BootStraper.IsMenuEnabled);
        }

        protected override void MenuEnabling(bool b)
        {
            _buttonMainSettings.Enabled = b;
            _buttonSettings.Enabled = b;
        }

        private void ButtonHelpOnOnClick(object sender, IRibbonControl control, bool pressed)
        {
            try
            {
                string helpurl = Properties.Settings.Default.HelpUrl;
                if (string.IsNullOrEmpty(helpurl))
                {
                    OFLogger.Instance.LogError("Run help: {0}", "Help url is empty");
                    return;
                }
                Process.Start(helpurl);
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError("Run help: {0}",ex.Message);
            }
        }

        private void ButtonAboutOnOnClick(object sender, IRibbonControl control, bool pressed)
        {
            OFAbout frmAbout = new OFAbout();
            frmAbout.ShowDialog();
        }

        private void ButtonSettingsOnOnClick(object sender, IRibbonControl control, bool pressed)
        {
            OFAddinModule.CurrentInstance.BootStraper.PassAction(new OFAction(OFActionType.Settings, null));
        }
    }
}