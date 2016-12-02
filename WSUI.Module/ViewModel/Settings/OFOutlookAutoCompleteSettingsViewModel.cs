using Microsoft.Practices.Unity;
using OF.Core.Core.MVVM;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;

namespace OF.Module.ViewModel.Settings
{
    public class OFOutlookAutoCompleteSettingsViewModel : OFViewModelBase,IOutlookAutoCompleteSettingsViewModel
    {
        private IUnityContainer _container;
        private IOutlookAutoCompleteSettingsView _view;

        public OFOutlookAutoCompleteSettingsViewModel(IUnityContainer container)
        {
            _container = container;
        }

        public void ApplySettings()
        {
            var officeVersion = OFRegistryHelper.Instance.GetOutlookVersion().Item1;
            if (IsAutoCompleateDisabled)
            {
                OFRegistryHelper.Instance.DisableAutoCompleateEmailsToCcBcc(officeVersion);
            }
            else
            {
                OFRegistryHelper.Instance.EnableAutoCompleateEmailsToCcBcc(officeVersion);
            }
        }

        public object View
        {
            get
            {
                if (_view.IsNull())
                {
                    _view = _container.Resolve<IOutlookAutoCompleteSettingsView>();
                    _view.Model = this;
                }
                return _view;
            }
        }
        
        public void Initialize()
        {
            Set(() => IsAutoCompleateDisabled,CheckAutoCompleateState());
        }

        public bool HasDetailsChanges
        {
            get { return Get(() => HasDetailsChanges); }
            set { Set(() => HasDetailsChanges,value);}
        }


        public bool IsAutoCompleateDisabled
        {
            get { return Get(() => IsAutoCompleateDisabled); }
            set
            {
                Set(() => IsAutoCompleateDisabled, value);
                HasDetailsChanges = true;
            }
        }

        private bool CheckAutoCompleateState()
        {
            var officeVersion = OFRegistryHelper.Instance.GetOutlookVersion().Item1;
            return OFRegistryHelper.Instance.IsOutlookAutoCompleateDisabled(officeVersion);
        }

    }
}