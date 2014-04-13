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
    [KindNameId(KindsConstName.Attachments, 3,
        @"pack://application:,,,/WSUI.Module;Component/Images/Mail-Attachment.png")]
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
            SearchSystem.Init();
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