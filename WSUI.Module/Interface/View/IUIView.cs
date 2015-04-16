namespace OF.Module.Interface.View
{
    interface IUView<T>
    {
        ISettingsView<T> SettingsView { get; set; }
        IDataView<T> DataView { get; set; }
    }
}
