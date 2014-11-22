using System;
using AddinExpress.MSO;

namespace WSUIOutlookPlugin.Interfaces
{
    public interface IWSUIOutlookItemEvents : IDisposable
    {
        bool ConnectTo(object olFolder, bool eventClassReleasesComObject);
        bool ConnectTo(object olFolder, bool eventClassReleasesComObject, bool recursive);
        bool ConnectTo(ADXOlDefaultFolders folderType, bool eventClassReleasesComObject);
        void RemoveConnection();
    }
}