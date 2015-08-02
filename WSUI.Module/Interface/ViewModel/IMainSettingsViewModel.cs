using System;

namespace OF.Module.Interface.ViewModel
{
    public interface IMainSettingsViewModel
    {
        object View { get; }

        event EventHandler Close;

        void Initialize();
    }
}