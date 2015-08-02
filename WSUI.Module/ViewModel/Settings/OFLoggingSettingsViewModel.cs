using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using OF.Core.Core.MVVM;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Logger;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;

namespace OF.Module.ViewModel.Settings
{
    public class OFLoggingSettingsViewModel : ViewModelBase,ILoggingSettingsViewModel
    {

        private IEventAggregator _eventAggregator;
        private IUnityContainer _unityContainer;

        public OFLoggingSettingsViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer)
        {
            _eventAggregator = eventAggregator;
            _unityContainer = unityContainer;
        }

        #region [properties]

        private ILoggingSettingsView _view;

        public object View
        {
            get
            {
                if (_view.IsNull())
                {
                    _view = _unityContainer.Resolve<ILoggingSettingsView>();
                    _view.Model = this;
                }
                return _view;
            }
        }

        public bool HasDetailsChanges { get {return HasChanges;} }

        public bool Info
        {
            get { return Get(() => Info); }
            set { Set(() => Info, value); }
        }

        public bool Warning
        {
            get { return Get(() => Warning); }
            set { Set(() => Warning, value); }
        }

        public bool Error
        {
            get { return Get(() => Error); }
            set { Set(() => Error, value); }
        }

        public bool Debug
        {
            get { return Get(() => Debug); }
            set { Set(() => Debug, value); }
        }


        #endregion


        #region [methods]

        public void ApplySettings()
        {
            int settings = 0;
            if (Info)
            {
                settings = settings | (int)OFLogger.LevelLogging.Info;
            }
            if (Warning)
            {
                settings = settings | (int) OFLogger.LevelLogging.Warning;
            }
            if (Error)
            {
                settings = settings | (int)OFLogger.LevelLogging.Error;
            }
            if (Debug)
            {
                settings = settings | (int)OFLogger.LevelLogging.Debug;
            }

            RegistryHelper.Instance.SetLoggingsettings(settings);
        }

        public void Initialize()
        {
            int settings = RegistryHelper.Instance.GetLoggingSettings();
            Info = OFLogger.LevelLogging.Info == (OFLogger.LevelLogging) ((int) OFLogger.LevelLogging.Info & settings);
            Warning = OFLogger.LevelLogging.Warning == (OFLogger.LevelLogging)((int)OFLogger.LevelLogging.Warning & settings);
            Error = OFLogger.LevelLogging.Error == (OFLogger.LevelLogging)((int)OFLogger.LevelLogging.Error & settings);
            Debug = OFLogger.LevelLogging.Debug == (OFLogger.LevelLogging)((int)OFLogger.LevelLogging.Debug & settings);
            HasChanges = false;
        }
         

        #endregion


        
    }
}