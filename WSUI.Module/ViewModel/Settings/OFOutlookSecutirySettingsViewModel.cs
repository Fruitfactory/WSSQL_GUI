using System;
using System.CodeDom;
using Microsoft.Practices.Unity;
using OF.Core.Core.MVVM;
using OF.Core.Data.Settings;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Logger;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;

namespace OF.Module.ViewModel.Settings
{
    public class OFOutlookSecutirySettingsViewModel : OFCoreSettingsViewModel, IOutlookSecuritySettingsViewModel
    {
        private IUnityContainer _container;
        private IOutlookSecuritySettingsView _view;

        public OFOutlookSecutirySettingsViewModel(IUnityContainer container)
        {
            _container = container;
        }
        
        public override void ApplySettings()
        {
            try
            {
                var officeVersion = OFRegistryHelper.Instance.GetOutlookVersion().Item1;
                if (IsSecurityWindowDisabled)
                {
                    OFRegistryHelper.Instance.DisableOutlookSecurityWarning(officeVersion, false);
                }
                else
                {
                    OFRegistryHelper.Instance.DeleteOutlookSecuritySettings(officeVersion, false);
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        public override object View
        {
            get
            {
                if (_view.IsNull())
                {
                    _view = _container.Resolve<IOutlookSecuritySettingsView>();
                    _view.Model = this;
                }
                return _view;
            }
        }

        public override void Initialize()
        {
            Set(() => IsSecurityWindowDisabled,CheckSecuritySettings());
        }

        public override bool HasDetailsChanges
        {
            get { return Get(() => HasDetailsChanges); }
            protected set { Set(() => HasDetailsChanges,value); }
        }


        public bool IsSecurityWindowDisabled
        {
            get { return Get(() => IsSecurityWindowDisabled); }
            set
            {
                Set(() => IsSecurityWindowDisabled, value);
                HasDetailsChanges = true;
            }
        }

        private bool CheckSecuritySettings()
        {
            var officeVersion = OFRegistryHelper.Instance.GetOutlookVersion().Item1;
            return OFRegistryHelper.Instance.IsSecurityWarningDisable(officeVersion) &&
                   OFRegistryHelper.Instance.IsAllSecuritySettingsApply(officeVersion);
        }


    }
}