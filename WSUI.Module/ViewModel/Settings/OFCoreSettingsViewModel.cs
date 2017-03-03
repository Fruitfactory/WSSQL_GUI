using OF.Core.Core.MVVM;
using OF.Core.Data.Settings;
using OF.Module.Interface.ViewModel;

namespace OF.Module.ViewModel.Settings
{
    public abstract class OFCoreSettingsViewModel : OFViewModelBase, IDetailsSettingsViewModel
    {
        public virtual bool IsRequiredAdminRights { get { return false; } }

        public virtual void ApplySettings() {}

        public virtual object View { get; protected set; }

        public virtual void Initialize() {}

        public virtual bool HasDetailsChanges { get; protected set; }

        public virtual OFTypeInspectionPayloadSettings GetAdminSettings()
        {
            return null;
        }
    }
}