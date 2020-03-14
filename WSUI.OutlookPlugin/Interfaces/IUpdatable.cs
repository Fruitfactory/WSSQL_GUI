namespace OFOutlookPlugin.Interfaces
{
    public interface IUpdatable
    {
		ThisAddIn Module { get; set; }
        bool IsUpdating();
        void RunSilentUpdate();
        bool CanUpdate();
        void Lock();
        void Unlock();
        void Update();
        void DeleteTempoparyFolders();
    }
}