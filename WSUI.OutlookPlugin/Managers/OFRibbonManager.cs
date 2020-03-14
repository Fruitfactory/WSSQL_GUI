using System;
using Microsoft.Office.Core;
using Microsoft.Practices.Prism.Events;
using OF.Core.Data;
using OF.Core.Enums;
using OF.Core.Events;
using OF.Core.Logger;
using OFOutlookPlugin.Core;

namespace OFOutlookPlugin.Managers
{
    public class OFRibbonManager : OFSearchCommandManager
    {
        //private readonly ADXRibbonButton _buttonSwitchView;
        //private readonly ADXRibbonButton _buttonSwitch;
        //private readonly ADXRibbonButton _buttonSearch;
        //private readonly ADXRibbonButton _buttonSearch1;
        //private readonly ADXRibbonEditBox _editCriteria;
        //private readonly ADXRibbonEditBox _homeEditCriteria;
        //private readonly ADXRibbonButton _buttonSendLog;

        public OFRibbonManager()
        {
            //_buttonSwitchView = btnSwitchView;
            //_buttonSwitch = btnSwitch;
            //_buttonSearch = btnSearch;
            //_editCriteria = edit;
            //_homeEditCriteria = homeEdit;
            //_buttonSearch1 = buttonSearch1;
            //_buttonSendLog = buttonSendLog;
            Init();
        }

        public override void SetShowHideButtonsEnabling(bool isShowButtonEnable, bool isHideButtonEnable)
        {
            //_buttonSwitchView.Enabled = isShowButtonEnable;
        }

        protected override void MenuEnabling(bool obj)
        {
            //_buttonSwitchView.Enabled = obj;
            //_buttonSwitch.Enabled = obj;
            //_buttonSearch.Enabled = obj;
            //_editCriteria.Enabled = obj;
            //_homeEditCriteria.Enabled = obj;
            //_buttonSearch1.Enabled = obj;
        }

        private void Init()
        {
            //_buttonSwitchView.OnClick += ButtonSwitchOnOnClick;
            //_buttonSwitch.OnClick += ButtonSwitchOnOnClick;
            //_homeEditCriteria.OnChange += HomeEditCriteriaOnOnChange;
            //_editCriteria.OnChange += HomeEditCriteriaOnOnChange;
            //_buttonSearch.OnClick += ButtonSearchOnOnClick;
            //_buttonSearch1.OnClick += ButtonMainSearchOnOnClick;
            //_buttonSendLog.OnClick += ButtonSendLogOnOnClick;
            //MenuEnabling(OFAddinModule.CurrentInstance.BootStraper.IsMenuEnabled);
        }

        private void ButtonSendLogOnOnClick(object sender, IRibbonControl control, bool pressed)
        {
            //OFAddinModule.CurrentInstance.BootStraper.PassAction(new OFAction(OFActionType.SendLogFile, null));
        }

        private void HomeEditCriteriaOnOnChange(object sender, IRibbonControl control, string text)
        {
            //var editBox = sender as ADXRibbonEditBox;
            //if(editBox == null)
            //    return;
            //InternalSearchPublich(editBox.Text);
            System.Diagnostics.Debug.WriteLine("Change Hone Criteria....");
        }

        private void ButtonMainSearchOnOnClick(object sender, IRibbonControl control, bool pressed)
        {
            
            //InternalSearchPublich(_homeEditCriteria.Text);
            System.Diagnostics.Debug.WriteLine("Home button search click....");
        }

        private void ButtonSearchOnOnClick(object sender, IRibbonControl control, bool pressed)
        {
            //InternalSearchPublich(_editCriteria.Text);
            System.Diagnostics.Debug.WriteLine("OutlookFinder button search click....");
        }

        private void ButtonSwitchOnOnClick(object sender, IRibbonControl control, bool pressed)
        {
            //if (OFAddinModule.CurrentInstance.IsMainUIVisible)
            //{
            //    InternalHidePublish();
            //}
            //else
            //{
            //    InternalShowPublish();
            //}
        }
    }
}