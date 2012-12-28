using System.IO;
using WSUI.Infrastructure.Service.Enums;
using WSUI.Infrastructure.Core;

namespace WSUI.Infrastructure.Service.Helpers
{
    public class SearchItemHelper
    {
        private const string FILEPREFIX = "file:";
        private const string FILEPREFIX1 = "file:///";
        private const string MAPIPREFIX = "mapi://";
        private const string ATSUFFIX = "/at=";

        #region public static 
        

        public static TypeSearchItem GetTypeItem(string itemurl,string tag = "")
        {
            if( tag == "contact")
                return TypeSearchItem.Contact;
            else if(tag == "calendar")
                return TypeSearchItem.Calendar;
            else if (itemurl.IndexOf(MAPIPREFIX) > -1 && itemurl.LastIndexOf(ATSUFFIX) > -1) 
            {
                return TypeSearchItem.Attachment;
            }
            else if (tag == "picture")
                return TypeSearchItem.Picture;
            else if (itemurl.IndexOf(FILEPREFIX1) > -1)
            {
                return TypeSearchItem.File;
            }
            else if (itemurl.IndexOf(FILEPREFIX) > -1)
            {
                return TypeSearchItem.File;
            }
            else if (itemurl.IndexOf(MAPIPREFIX) > -1)
            {
                return TypeSearchItem.Email;
            }
            

            return TypeSearchItem.None;
        }

        public static string GetFileName(BaseSearchData item, bool forPreview = true)
        {
            if (item == null)
                return null;

            switch(item.Type)
            {
                case TypeSearchItem.Email:
                    return OutlookHelper.Instance.GetEMailTempFileName(item);
                case TypeSearchItem.Attachment:
                    return OutlookHelper.Instance.GetAttachmentTempFileName(item);
                case TypeSearchItem.Calendar:
                    return OutlookHelper.Instance.GetCalendarTempFileName(item);    
                case TypeSearchItem.File:
                case TypeSearchItem.Picture:
                    return forPreview ? GetNormalizeFilename(item, GetFileName(item.Path)) : GetFileName(item.Path);
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

        private static string GetNormalizeFilename(BaseSearchData item, string filename)
        {
            var ext = Path.GetExtension(filename);
            string normalFilename = string.Empty;
            switch (ext)
            {
                case ".pdf":
                    normalFilename = TempFileManager.Instance.GenerateTempFileNameWithCopy(item, filename);
                    break;
                default:
                    normalFilename = filename;
                    break;
            }
            return normalFilename;
        }


        #endregion

    }
}
