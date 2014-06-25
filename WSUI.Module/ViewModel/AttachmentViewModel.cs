using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Unity;
using WSUI.Core.Enums;
using WSUI.Infrastructure.Implements.Systems;
using WSUI.Infrastructure.Service;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using WSUI.Module.Service;
using WSUI.Module.Strategy;

namespace WSUI.Module.ViewModel
{
    [KindNameId(KindsConstName.Attachments, 3, @"pack://application:,,,/WSUI.Module;Component/Images/Mail-Attachment.png", "M0.63598693,10.206985L16.182993,25.755979C16.695994,26.266967,17.526011,26.266967,18.039013,25.755979L33.526997,10.266922C33.844013,10.477982,34.052998,10.83394,34.052998,11.2419L34.052998,31.176038C34.052998,31.823989,33.531025,32.344985,32.887043,32.344985L1.1669928,32.344985C0.52099639,32.344985,0,31.823989,0,31.176038L0,11.2419C0,10.78792,0.25897232,10.400956,0.63598693,10.206985z M22.819899,1.4982157C20.330864,1.5162153,19.097847,2.9362087,19.115847,5.7611952L19.131848,8.2671828C19.28385,9.1821783,20.302864,9.9391746,21.679882,10.228173L21.643883,4.9841986C21.638882,4.292202 22.19389,3.7272048 22.8839,3.7232046 23.578909,3.7192049 24.145916,4.2752023 24.148916,4.9661989L24.183918,10.221173C25.662937,9.9041747 26.720953,9.0431789 26.720953,8.0321836 26.720953,8.0131838 26.723953,7.9921839 26.727953,7.9731839L26.711952,5.8551946C26.692951,2.9342084,25.393934,1.480216,22.819899,1.4982157z M22.993901,0.00022315979C26.608952,-0.023777008,28.432976,1.890214,28.457976,5.7331948L28.470976,7.4831865 31.787022,7.5241861C31.771023,7.8011849,31.656021,8.0771837,31.439018,8.2911828L17.87883,21.775117C17.424824,22.220115,16.705815,22.221115,16.275809,21.781117L2.7476215,8.3561823C2.5246189,8.1291833,2.4246175,7.8241849,2.4406178,7.5241861L17.381823,7.5601864 17.373823,6.0411935C17.344824,2.0382128,19.218849,0.024222374,22.993901,0.00022315979z")]
    public class AttachmentViewModel : KindViewModelBase, IUView<AttachmentViewModel>, IScrollableView
    {
        public AttachmentViewModel(IUnityContainer container, ISettingsView<AttachmentViewModel> settingsView,
            IDataView<AttachmentViewModel> dataView)
            : base(container)
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
            ICommandStrategy fileAttach = CommadStrategyFactory.CreateStrategy(TypeSearchItem.FileAll, this);
            CommandStrategies.Add(TypeSearchItem.Attachment, fileAttach);
            CommandStrategies.Add(TypeSearchItem.FileAll, fileAttach);
            CommandStrategies.Add(TypeSearchItem.File, fileAttach);
            CommandStrategies.Add(TypeSearchItem.Picture, fileAttach);
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