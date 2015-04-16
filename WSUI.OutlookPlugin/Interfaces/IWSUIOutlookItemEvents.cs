using System;
using AddinExpress.MSO;

namespace OFOutlookPlugin.Interfaces
{
    public interface IOFOutlookItemEvents : IDisposable
    {
        bool ConnectTo(object olFolder, bool eventClassReleasesComObject);
        bool ConnectTo(object olFolder, bool eventClassReleasesComObject, bool recursive);
        bool ConnectTo(ADXOlDefaultFolders folderType, bool eventClassReleasesComObject);
        void RemoveConnection();
    }
}