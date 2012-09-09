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

        #region public static 
        

        public static TypeSearchItem GetTypeItem(string itemurl)
        {
            int index = -1;
            if ((index = itemurl.IndexOf(FILEPREFIX1)) > -1)
            {
                return TypeSearchItem.File;
            }
            else if ((index = itemurl.IndexOf(FILEPREFIX)) > -1)
            {
                return TypeSearchItem.File;
            }
            else if ((index = itemurl.IndexOf(MAPIPREFIX)) > -1)
            {
                return TypeSearchItem.Email;
            }
            return TypeSearchItem.None;
        }

        public static string GetFileName(SearchItem item)
        {
            switch(item.Type)
            {
                case TypeSearchItem.Email:
                    string temp = item.IsAttachment ? OutlookHelper.Instance.GetAttachmentTempFileName(item) :
                                                      OutlookHelper.Instance.GetEMailTempFileName(item);
                    return temp;
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
