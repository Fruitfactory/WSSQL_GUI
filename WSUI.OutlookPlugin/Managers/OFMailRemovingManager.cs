using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AddinExpress.MSO;
using OF.Control;
using OF.Core.Data;
using OF.Core.Enums;
using OF.Core.Extensions;
using OFOutlookPlugin.Interfaces;
using OFOutlookPlugin.Managers.OutlookEventsManagers;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace OFOutlookPlugin.Managers
{
    public class OFMailRemovingManager : IOFMailRemovingManager
    {
        #region [needs]

        private readonly IDictionary<string,object> _dictionaryDeleteFolders = new Dictionary<string, object>();


        private OFAddinModule _module;
        private IDictionary<string,Outlook.Folder> _folders = new Dictionary<string, Outlook.Folder>();
        private OFOutlookItemEvents _itemEvents;



        #endregion

        public OFMailRemovingManager(OFAddinModule Module)
        {
            _module = Module;
            _itemEvents = new OFOutlookItemEvents(_module,this);
        }

        public void Initialize()
        {
            var nm = _module.OutlookApp.GetNamespace("MAPI");
            if (nm.IsNotNull())
            {
                for(int i = 1; i <= nm.Stores.Count; i++)
                {
                    var store = nm.Stores[i];
                    Outlook.PropertyAccessor pa = store.PropertyAccessor;
                    var property = (byte[])pa.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x35E30102");
                    var id = pa.BinaryToString(property);
                    Outlook.MAPIFolder folder = null;
                    if (!string.IsNullOrEmpty(id) && (folder = nm.GetFolderFromID(id)) != null)
                    {
                        _dictionaryDeleteFolders.Add(folder.FullFolderPath,null);
                    }
                    if (pa.IsNotNull())
                    {
                        Marshal.ReleaseComObject(pa);
                    }
                    if (folder.IsNotNull())
                    {
                        Marshal.ReleaseComObject(folder);
                    }
                    if (store.IsNotNull())
                    {
                        Marshal.ReleaseComObject(store);
                    }
                }
            }
        }

        public void RemoveMail(Outlook.MailItem Mail)
        {
            InternalRemoveMail(Mail, null);
        }


        public void ConnectTo(Outlook.Folder Folder)
        {
            if (Folder.IsNull() )
            {
                return;
            }

            if (!_folders.ContainsKey(Folder.EntryID))
            {
                Outlook.Folder folder = _module.OutlookApp.Session.GetFolderFromID(Folder.EntryID) as Outlook.Folder;
                if (folder != null)
                {
                    folder.BeforeItemMove += FolderOnBeforeItemMove;
                    _folders.Add(Folder.EntryID,folder);
                }
            }
        }

        public void RemoveConnection()
        {
            _itemEvents.RemoveConnection();
        }

        public void ConnectTo(Outlook.MailItem MailItem)
        {
            _itemEvents.ConnectTo(MailItem, true);
        }


        private void FolderOnBeforeItemMove(object item, Outlook.MAPIFolder moveTo, ref bool cancel)
        {
            InternalRemoveMail(item, moveTo);
        }

        private void InternalRemoveMail(object item, Outlook.MAPIFolder moveTo)
        {
            Outlook.MailItem mail = item as Outlook.MailItem;
            if (mail.IsNull())
            {
                return;
            }
            if (moveTo == null || (moveTo.IsNotNull() && _dictionaryDeleteFolders.ContainsKey(moveTo.FullFolderPath)))
            {
                _module.BootStraper.PassAction(new OFAction(OFActionType.DeleteMail, mail.EntryID));
            }
        }


        public void Dispose()
        {
            if (_folders.Any())
            {
                foreach (var folder in _folders)
                {
                    folder.Value.BeforeItemMove -= FolderOnBeforeItemMove;
                    Marshal.ReleaseComObject(folder.Value);
                }
                _folders.Clear();
            }
            _itemEvents.RemoveConnection();
            _itemEvents.Dispose();
        }
    }
}