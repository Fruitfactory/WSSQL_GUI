using System;

namespace OF.Module.Interface.ViewModel
{
    public interface IElasticSearchRiverSettingsViewModel
    {
        object View { get; }

        event EventHandler Close;

        void Initialize();
    }
}