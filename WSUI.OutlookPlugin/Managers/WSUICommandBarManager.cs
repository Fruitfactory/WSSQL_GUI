using System;
using AddinExpress.MSO;
using WSUI.Core.Logger;
using WSUIOutlookPlugin.Core;

namespace WSUIOutlookPlugin.Managers
{
    public class WSUICommandBarManager : SearchCommandManager
    {
        private readonly ADXCommandBarButton _buttonShow;
        private readonly ADXCommandBarButton _buttonHide;
        private readonly ADXCommandBarEdit _editCriteria;
        private readonly ADXCommandBarButton _buttonSearch;

        public WSUICommandBarManager(ADXCommandBarButton btnShow, ADXCommandBarButton btnHide,
            ADXCommandBarEdit edit, ADXCommandBarButton btnSearch)
        {
            _buttonShow = btnShow;
            _buttonHide = btnHide;
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
            _buttonShow.Click += ButtonShowOnClick;
            _buttonHide.Click += ButtonHideOnClick;
            _editCriteria.Change += EditCriteriaOnChange;
            _buttonSearch.Click += ButtonSearchOnClick;
            _buttonShow.Enabled = true;
            _buttonHide.Enabled = false;
        }

        private void EditCriteriaOnChange(object sender)
        {
            InternalSearchPublich(_editCriteria.Text);
            ApplyCommandBarButtons(false);
        }

        private void ButtonSearchOnClick(object sender)
        {
            InternalSearchPublich(_editCriteria.Text);
            ApplyCommandBarButtons(false);
        }

        private void ButtonHideOnClick(object sender)
        {
            InternalHidePublish();
            ApplyCommandBarButtons();
        }

        private void ButtonShowOnClick(object sender)
        {
            InternalShowPublish();
            ApplyCommandBarButtons(false);
        }

        private void ApplyCommandBarButtons(bool isOpenEnable = true)
        {
            _buttonShow.Enabled = isOpenEnable;
            _buttonHide.Enabled = !isOpenEnable;
        }
    }
}