﻿namespace OFOutlookPlugin.Interfaces
{
    public interface IUpdatable
    {
        AddinExpress.MSO.ADXAddinModule Module { get; set; }
        bool IsUpdating();
        void RunSilentUpdate();
        bool CanUpdate();
        void Lock();
        void Unlock();
        void Update();
        void DeleteTempoparyFolders();
    }
}