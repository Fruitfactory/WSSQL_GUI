using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using WSUI.Core.Enums;
using WSUI.Infrastructure.Implements.Systems;
using WSUI.Infrastructure.Service;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using WSUI.Module.Interface.Service;
using WSUI.Module.Interface.View;
using WSUI.Module.Service;
using WSUI.Module.Strategy;

namespace WSUI.Module.ViewModel
{
    [KindNameId(KindsConstName.Attachments, 3, @"pack://application:,,,/WSUI.Module;Component/Images/Mail-Attachment.png", "F1M-1800.12,3181.48C-1800.12,3181.48 -1803.22,3172.42 -1813.67,3175.58 -1813.67,3175.58 -1822.52,3177.87 -1820.93,3188.08L-1807.87,3226.17 -1804.99,3225.18 -1817.71,3188.05C-1817.71,3188.05 -1820.44,3181.75 -1812.98,3178.38 -1812.98,3178.38 -1805.77,3175.6 -1802.56,3183.22L-1786.65,3229.63C-1786.65,3229.63 -1786.14,3234.44 -1790.63,3235.46 -1790.63,3235.46 -1792.78,3236.54 -1796.5,3233.79L-1803.82,3212.42 -1809.85,3194.84C-1809.85,3194.84 -1809.94,3192.33 -1808.27,3191.61 -1807.43,3191.25 -1805.9,3191.14 -1805.05,3193.37L-1794.19,3225.06 -1791.09,3224 -1802.17,3191.67C-1802.17,3191.67 -1803.96,3187.16 -1809.04,3188.84 -1810.81,3189.42 -1813.38,3190.48 -1812.95,3195.18L-1799.38,3234.78C-1799.38,3234.78 -1796.27,3240.08 -1789.53,3238.67 -1789.53,3238.67 -1783.39,3237.06 -1783.69,3229.41L-1800.12,3181.48z")]
    public class AttachmentViewModel : KindViewModelBase, IUView<AttachmentViewModel>, IScrollableView
    {
        public AttachmentViewModel(IUnityContainer container, IEventAggregator eventAggregator, ISettingsView<AttachmentViewModel> settingsView,
            IDataView<AttachmentViewModel> dataView)
            : base(container,eventAggregator)
        {
            SettingsView = settingsView;
            SettingsView.Model = this;
            DataView = dataView;
            DataView.Model = this;
            ID = 3;
            _name = "Attachments";
            UIName = _name;
            _prefix = "Attachment";
            ScrollChangeCommand = new DelegateCommand<object>(OnScroll, o => true);

            SearchSystem = new AttachmentSearchSystem();
        }

        #region IUView<AttachmentViewModel> Members

        public ISettingsView<AttachmentViewModel> SettingsView { get; set; }

        public IDataView<AttachmentViewModel> DataView { get; set; }

        #endregion IUView<AttachmentViewModel> Members

        #region Implementation of IScrollableView

        public ICommand ScrollChangeCommand { get; private set; }

        #endregion Implementation of IScrollableView

        protected override void OnInit()
        {
            base.OnInit();
            ScrollBehavior = new ScrollBehavior {CountFirstProcess = 100, CountSecondProcess = 50, LimitReaction = 75};
            ScrollBehavior.SearchGo += OnScrollNeedSearch;
        }

        private void OnScroll(object args)
        {
            var scrollArgs = args as ScrollData;
            if (scrollArgs != null && ScrollBehavior != null)
            {
                ScrollBehavior.NeedSearch(scrollArgs);
            }
        }
    }
}