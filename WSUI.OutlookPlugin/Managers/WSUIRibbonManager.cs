using AddinExpress.MSO;
using WSUI.Core.Logger;
using WSUIOutlookPlugin.Core;

namespace WSUIOutlookPlugin.Managers
{
    public class WSUIRibbonManager : SearchCommandManager
    {
        private readonly ADXRibbonButton _buttonSwitchView;
        private readonly ADXRibbonButton _buttonSwitch;
        private readonly ADXRibbonButton _buttonSearch;
        private readonly ADXRibbonButton _buttonSearch1;
        private readonly ADXRibbonEditBox _editCriteria;
        private readonly ADXRibbonEditBox _homeEditCriteria;

        public WSUIRibbonManager(ADXRibbonButton btnSwitchView,
            ADXRibbonButton btnSwitch, ADXRibbonButton btnSearch, ADXRibbonEditBox edit, ADXRibbonEditBox homeEdit,ADXRibbonButton buttonSearch1)
        {
            _buttonSwitchView = btnSwitchView;
            _buttonSwitch = btnSwitch;
            _buttonSearch = btnSearch;
            _editCriteria = edit;
            _homeEditCriteria = homeEdit;
            _buttonSearch1 = buttonSearch1;
            Init();
        }

        public override void SetShowHideButtonsEnabling(bool isShowButtonEnable, bool isHideButtonEnable)
        {
            _buttonSwitchView.Enabled = isShowButtonEnable;
        }

        private void Init()
        {
            _buttonSwitchView.OnClick += ButtonSwitchOnOnClick;
            _buttonSwitch.OnClick += ButtonSwitchOnOnClick;
            _homeEditCriteria.OnChange += HomeEditCriteriaOnOnChange;
            _editCriteria.OnChange += HomeEditCriteriaOnOnChange;
            _buttonSearch.OnClick += ButtonSearchOnOnClick;
            _buttonSearch1.OnClick += ButtonMainSearchOnOnClick;
        }

        private void HomeEditCriteriaOnOnChange(object sender, IRibbonControl control, string text)
        {
            var editBox = sender as ADXRibbonEditBox;
            if(editBox == null)
                return;
            InternalSearchPublich(editBox.Text);
            System.Diagnostics.Debug.WriteLine("Change Hone Criteria....");
        }

        private void ButtonMainSearchOnOnClick(object sender, IRibbonControl control, bool pressed)
        {
            
            InternalSearchPublich(_homeEditCriteria.Text);
            System.Diagnostics.Debug.WriteLine("Home button search click....");
        }

        private void ButtonSearchOnOnClick(object sender, IRibbonControl control, bool pressed)
        {
            InternalSearchPublich(_editCriteria.Text);
            System.Diagnostics.Debug.WriteLine("OutlookFinder button search click....");
        }

        private void ButtonSwitchOnOnClick(object sender, IRibbonControl control, bool pressed)
        {
            if (WSUIAddinModule.CurrentInstance.IsMainUIVisible)
            {
                InternalHidePublish();
            }
            else
            {
                InternalShowPublish();
            }
        }
    }
}