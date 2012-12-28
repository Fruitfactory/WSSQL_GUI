namespace WSUI.Module.Interface
{
    interface IUView<T>
    {
        ISettingsView<T> SettingsView { get; set; }
        IDataView<T> DataView { get; set; }
    }
}
