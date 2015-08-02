using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using OF.Core.Core.MVVM;
using OF.Core.Extensions;
using OF.Core.Utils.Dialog;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;

namespace OF.Module.ViewModel.Settings
{
    public class OFMainSettingsViewModel : ViewModelBase, IMainSettingsViewModel
    {
        private IUnityContainer _unityContainer;
        private IEventAggregator _eventAggregator;

        private List<IDetailsSettingsViewModel> _detailsSettingsViewModels; 


        #region [ctor]

        public OFMainSettingsViewModel(IUnityContainer unityContainer, IEventAggregator eventAggregator)
        {
            _unityContainer = unityContainer;
            _eventAggregator = eventAggregator;
            _detailsSettingsViewModels = new List<IDetailsSettingsViewModel>();
        }

        #endregion

        #region [properties]

        private IMainSettingsView _view = null;
        public object View
        {
            get
            {
                if (_view.IsNull())
                {
                    _view = _unityContainer.Resolve<IMainSettingsView>();
                    _view.Model = this;
                }
                return _view;
            }
        }

        public event EventHandler Close;

        public void Initialize()
        {
            _detailsSettingsViewModels.Add(_unityContainer.Resolve<IElasticSearchRiverSettingsViewModel>());
            _detailsSettingsViewModels.Add(_unityContainer.Resolve<ILoggingSettingsViewModel>());

            _detailsSettingsViewModels.ForEach(d => d.Initialize());

            OkCommand = new OFRelayCommand(OkCommandExecute,CanOkCommandExecute);
            CancelCommand = new OFRelayCommand(CancelCommandExecute);
        }

        public IElasticSearchRiverSettingsViewModel RiverSettingsViewModel
        {
            get { return _detailsSettingsViewModels[0] as IElasticSearchRiverSettingsViewModel;}        
        }

        public ILoggingSettingsViewModel LoggingSettingsViewModel
        {
            get { return _detailsSettingsViewModels[1] as ILoggingSettingsViewModel;}
        }

        public ICommand OkCommand
        {
            get { return Get(() => OkCommand); }
            private set { Set(() => OkCommand, value); }
        }

        public ICommand CancelCommand
        {
            get { return Get(() => CancelCommand); }
            private set { Set(() => CancelCommand, value); }
        }

        #endregion

        #region [methods]

        private void OkCommandExecute(object args)
        {
            _detailsSettingsViewModels.ForEach(d => d.ApplySettings());
            CloseRaise();
        }

        private bool CanOkCommandExecute(object args)
        {
            return _detailsSettingsViewModels.Any(d => d.HasDetailsChanges);
        }

        private void CloseRaise()
        {
            EventHandler temp = Close;
            if (temp.IsNotNull())
            {
                Close(this, EventArgs.Empty);
            }
        }

        private void CancelCommandExecute(object args)
        {
            CloseRaise();
        }


        #endregion

    }
}