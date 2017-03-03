using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using OF.Core.Core.MVVM;
using OF.Core.Data.Settings;
using OF.Core.Data.Settings.SettingsPayload;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;

namespace OF.Module.ViewModel.Settings
{
    public class OFOutlookAutoCompleteSettingsViewModel : OFCoreSettingsViewModel, IOutlookAutoCompleteSettingsViewModel
    {
        private IUnityContainer _container;
        private IOutlookAutoCompleteSettingsView _view;

        public OFOutlookAutoCompleteSettingsViewModel(IUnityContainer container)
        {
            _container = container;
        }

        public override bool IsRequiredAdminRights { get { return true; } }

        public override OFTypeInspectionPayloadSettings GetAdminSettings()
        {
            return new OFTypeInspectionPayloadSettings()
            {
                Type = OFSettingsType.AutoComplete,
                SettingsPayload = JsonConvert.SerializeObject(new OFAutoCompleteSettingsPayload()
                {
                    IsAutoCompleateDisabled = this.IsAutoCompleateDisabled
                }, 
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.None
                })
            };
        }

        public override object View
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

        public override void Initialize()
        {
            Set(() => IsAutoCompleateDisabled, CheckAutoCompleateState());
        }

        public override bool HasDetailsChanges
        {
            get { return Get(() => HasDetailsChanges); }
            protected set { Set(() => HasDetailsChanges, value); }
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