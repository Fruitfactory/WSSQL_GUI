using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class OFMainSettingsViewModel : OFViewModelBase, IMainSettingsViewModel
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
            _detailsSettingsViewModels.Add(_unityContainer.Resolve<IServiceApplicationSettingsViewModel>());
            _detailsSettingsViewModels.Add(_unityContainer.Resolve<IOutlookSecuritySettingsViewModel>());

            if (_detailsSettingsViewModels[2].IsNotNull())
            {
                _detailsSettingsViewModels[2].PropertyChanged += OnPropertyChanged;
            }


            _detailsSettingsViewModels.ForEach(d => d.Initialize());

            SelectedTab = IsIndexTabEnabled ? 0 : 1;

            OkCommand = new OFRelayCommand(OkCommandExecute,CanOkCommandExecute);
            CancelCommand = new OFRelayCommand(CancelCommandExecute);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            OnPropertyChanged("");
        }

        public IElasticSearchRiverSettingsViewModel RiverSettingsViewModel
        {
            get { return _detailsSettingsViewModels[0] as IElasticSearchRiverSettingsViewModel;}        
        }

        public ILoggingSettingsViewModel LoggingSettingsViewModel
        {
            get { return _detailsSettingsViewModels[1] as ILoggingSettingsViewModel;}
        }

        public IServiceApplicationSettingsViewModel ServiceApplicationSettingsViewModel
        {
            get { return _detailsSettingsViewModels[2] as IServiceApplicationSettingsViewModel;}
        }

        public IOutlookSecuritySettingsViewModel OutlookSecuritySettingsViewModel
        {
            get { return _detailsSettingsViewModels[3] as IOutlookSecuritySettingsViewModel;}
        }

        public int SelectedTab
        {
            get { return Get(() => SelectedTab); }
            set { Set(() => SelectedTab, value); }
        }

        public bool IsIndexTabEnabled
        {
            get
            {
                return ServiceApplicationSettingsViewModel.IsElasticSearchServiceRunning &&
                       ServiceApplicationSettingsViewModel.IsElasticSearchServiceInstalled;
            }
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