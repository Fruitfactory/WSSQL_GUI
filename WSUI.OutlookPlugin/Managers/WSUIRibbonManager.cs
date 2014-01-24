using System;
using System.Text;
using System.Windows.Input;
using AddinExpress.MSO;
using WSUI.Core.Win32;
using WSUIOutlookPlugin.Core;

namespace WSUIOutlookPlugin.Managers
{
    public class WSUIRibbonManager : BaseCommandManager
    {
        private const string ClassName = "RICHEDIT60W";
        private const int EnterKey = 13;

        private readonly ADXRibbonButton _buttonShow;
        private readonly ADXRibbonButton _buttonHide;
        private readonly ADXRibbonButton _buttonSwitch;
        private readonly ADXRibbonButton _buttonSearch;
        private readonly ADXRibbonButton _buttonSearch1;
        private readonly ADXRibbonEditBox _editCriteria;
        private readonly ADXRibbonEditBox _homeEditCriteria;
        
        private string _oldEditCriteria = string.Empty;
        private string _oldHomeEditCriteria = string.Empty;

        public WSUIRibbonManager(ADXRibbonButton btnShow, ADXRibbonButton btnHide, 
            ADXRibbonButton btnSwitch, ADXRibbonButton btnSearch, ADXRibbonEditBox edit, ADXRibbonEditBox homeEdit,ADXRibbonButton buttonSearch1)
        {
            _buttonShow = btnShow;
            _buttonHide = btnHide;
            _buttonSwitch = btnSwitch;
            _buttonSearch = btnSearch;
            _editCriteria = edit;
            _homeEditCriteria = homeEdit;
            _buttonSearch1 = buttonSearch1;
            Init();
        }

        public override void SetShowHideButtonsEnabling(bool isShowButtonEnable, bool isHideButtonEnable)
        {
            _buttonShow.Enabled = isShowButtonEnable;
            _buttonHide.Enabled = isHideButtonEnable;
        }

        private void Init()
        {
            _buttonShow.OnClick += ButtonShowOnOnClick;
            _buttonHide.OnClick += ButtonHideOnOnClick;
            _buttonSwitch.OnClick += ButtonSwitchOnOnClick;
            _homeEditCriteria.OnChange += HomeEditCriteriaOnOnChange;
            _editCriteria.OnChange += HomeEditCriteriaOnOnChange;
            _buttonSearch.OnClick += ButtonSearchOnOnClick;
            _buttonSearch1.OnClick += ButtonMainSearchOnOnClick;
            _buttonShow.Enabled = true;
            _buttonHide.Enabled = false;
        }

        private void HomeEditCriteriaOnOnChange(object sender, IRibbonControl control, string text)
        {
            var editBox = sender as ADXRibbonEditBox;
            if(editBox == null)
                return;
            InternalSearchPublich(editBox.Text);
            ApplyButtonsRibbonEnable(false);
            System.Diagnostics.Debug.WriteLine("Change Hone Criteria....");
        }

        private void ButtonMainSearchOnOnClick(object sender, IRibbonControl control, bool pressed)
        {
            InternalSearchPublich(_homeEditCriteria.Text);
            ApplyButtonsRibbonEnable(false);
            System.Diagnostics.Debug.WriteLine("Home button search click....");
        }

        private void ButtonSearchOnOnClick(object sender, IRibbonControl control, bool pressed)
        {
            InternalSearchPublich(_editCriteria.Text);
            ApplyButtonsRibbonEnable(false);
            System.Diagnostics.Debug.WriteLine("OutlookFinder button search click....");
        }

        private void ButtonSwitchOnOnClick(object sender, IRibbonControl control, bool pressed)
        {
            if (WSUIAddinModule.CurrentInstance.IsMainUIVisible)
            {
                InternalHidePublish();
                ApplyButtonsRibbonEnable();
            }
            else
            {
                InternalShowPublish();
                ApplyButtonsRibbonEnable(false);
            }
        }

        private void ButtonHideOnOnClick(object sender, IRibbonControl control, bool pressed)
        {
            InternalHidePublish();
            ApplyButtonsRibbonEnable();
        }

        private void ButtonShowOnOnClick(object sender, IRibbonControl control, bool pressed)
        {
            InternalShowPublish();
            ApplyButtonsRibbonEnable(false);
        }

        private void ApplyButtonsRibbonEnable(bool isShowEnable = true)
        {
            _buttonShow.Enabled = isShowEnable;
            _buttonHide.Enabled = !isShowEnable;
        }
    }
}