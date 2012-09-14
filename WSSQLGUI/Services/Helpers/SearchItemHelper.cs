using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSSQLGUI.Services.Enums;
using WSSQLGUI.Models;

namespace WSSQLGUI.Services.Helpers
{
    class SearchItemHelper
    {
        private const string FILEPREFIX = "file:";
        private const string FILEPREFIX1 = "file:///";
        private const string MAPIPREFIX = "mapi://";
        private const string ATSUFFIX = "/at=";

        #region public static 
        

        public static TypeSearchItem GetTypeItem(string itemurl)
        {
            if (itemurl.IndexOf(FILEPREFIX1) > -1)
            {
                return TypeSearchItem.File;
            }
            else if (itemurl.IndexOf(FILEPREFIX) > -1)
            {
                return TypeSearchItem.File;
            }
            else if (itemurl.IndexOf(MAPIPREFIX) > -1 && itemurl.LastIndexOf(ATSUFFIX) > -1) 
            {
                return TypeSearchItem.Attachment;
            }
            else if (itemurl.IndexOf(MAPIPREFIX) > -1)
            {
                return TypeSearchItem.Email;
            }
            return TypeSearchItem.None;
        }

        public static string GetFileName(SearchItem item)
        {
            if (item == null)
                return null;

            switch(item.Type)
            {
                case TypeSearchItem.Email:
                    return OutlookHelper.Instance.GetEMailTempFileName(item);
                case TypeSearchItem.Attachment:
                    return OutlookHelper.Instance.GetAttachmentTempFileName(item);
                case TypeSearchItem.File:
                    return GetFileName(item.FileName);
                default:
                    return null;
            }
        }

        #endregion

        #region private static

        private static string GetFileName(string path)
        {
            int index = -1;
            if ((index = path.IndexOf(FILEPREFIX1)) > -1)
            {
                return path.Substring(index + FILEPREFIX1.Length);
            }
            else if ((index = path.IndexOf(FILEPREFIX)) > -1)
            {
                return path.Substring(index + FILEPREFIX.Length);
            }
            return null;
        }

        #endregion

    }
}
