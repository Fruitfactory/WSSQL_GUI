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

        public WSUIRibbonManager(ADXRibbonButton btnShow, ADXRibbonButton btnHide, 
            ADXRibbonButton btnSwitch, ADXRibbonButton btnSearch, ADXRibbonEditBox edit)
        {
            _buttonShow = btnShow;
            _buttonHide = btnHide;
            _buttonSwitch = btnSwitch;
            _buttonSearch = btnSearch;
            _editCriteria = edit;
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
            _buttonShow.Enabled = true;
            _buttonHide.Enabled = false;
        }

        private void EditCriteriaOnOnChange(object sender, IRibbonControl control, string text)
        {
            InternalSearchPublich(_editCriteria.Text);
        }

        private void ButtonSearchOnOnClick(object sender, IRibbonControl control, bool pressed)
        {
            InternalSearchPublich(_editCriteria.Text);
        }

        private void ButtonSwitchOnOnClick(object sender, IRibbonControl control, bool pressed)
        {
                                
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