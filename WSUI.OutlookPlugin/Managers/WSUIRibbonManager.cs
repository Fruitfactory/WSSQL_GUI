using System;
using AddinExpress.MSO;
using WSUIOutlookPlugin.Core;

namespace WSUIOutlookPlugin.Managers
{
    public class WSUIRibbonManager : BaseCommandManager
    {
        private readonly ADXRibbonButton _buttonShow;
        private readonly ADXRibbonButton _buttonHide;
        private readonly ADXRibbonButton _buttonSwitch;
        private readonly ADXRibbonButton _buttonSearch;
        private readonly ADXRibbonEditBox _editCriteria;
        private readonly ADXRibbonEditBox _homeEditCriteria;
        
        public WSUIRibbonManager(ADXRibbonButton btnShow, ADXRibbonButton btnHide, 
            ADXRibbonButton btnSwitch, ADXRibbonButton btnSearch, ADXRibbonEditBox edit, ADXRibbonEditBox homeEdit)
        {
            _buttonShow = btnShow;
            _buttonHide = btnHide;
            _buttonSwitch = btnSwitch;
            _buttonSearch = btnSearch;
            _editCriteria = edit;
            _homeEditCriteria = homeEdit;
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
            _buttonSearch.OnClick += ButtonSearchOnOnClick;
            _editCriteria.OnChange += EditCriteriaOnOnChange;
            _homeEditCriteria.OnChange += EditCriteriaOnOnChange;
            _buttonShow.Enabled = true;
            _buttonHide.Enabled = false;
        }

        private void EditCriteriaOnOnChange(object sender, IRibbonControl control, string text)
        {
            var editBox = sender as ADXRibbonEditBox;
            if (editBox == null)
                return;
            InternalSearchPublich(editBox.Text);
        }

        private void ButtonSearchOnOnClick(object sender, IRibbonControl control, bool pressed)
        {
            InternalSearchPublich(_editCriteria.Text);
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