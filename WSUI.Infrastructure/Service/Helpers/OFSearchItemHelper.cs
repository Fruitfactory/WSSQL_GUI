using System.IO;
using OF.Core.Core;
using OF.Core.Data;
using OF.Core.Enums;
using OF.Core.Helpers;

namespace OF.Infrastructure.Service.Helpers
{
    public class OFSearchItemHelper
    {
        private const string FILEPREFIX = "file:";
        private const string FILEPREFIX1 = "file:///";
        private const string MAPIPREFIX = "mapi://";
        private const string ATSUFFIX = "/at=";

        #region public static 
        

        public static OFTypeSearchItem GetTypeItem(string itemurl,string tag = "")
        {
            if( tag == "contact")
                return OFTypeSearchItem.Contact;
            else if(itemurl.IndexOf(MAPIPREFIX) > -1 && itemurl.LastIndexOf(ATSUFFIX) > -1)
                return OFTypeSearchItem.Attachment;
            else if (tag == "calendar" && itemurl.IndexOf(FILEPREFIX) > -1)
            {
                return OFTypeSearchItem.File;
            }
            else if (tag == "calendar")
            {
                return OFTypeSearchItem.Calendar;
            }
            else if (tag == "picture")
                return OFTypeSearchItem.Picture;
            else if (itemurl.IndexOf(FILEPREFIX1) > -1)
            {
                return OFTypeSearchItem.File;
            }
            else if (itemurl.IndexOf(FILEPREFIX) > -1)
            {
                return OFTypeSearchItem.File;
            }
            else if (itemurl.IndexOf(MAPIPREFIX) > -1)
            {
                return OFTypeSearchItem.Email;
            }
            

            return OFTypeSearchItem.None;
        }

        public static string GetFileName(OFBaseSearchObject item, bool forPreview = true)
        {
            if (item == null)
                return null;

            switch(item.TypeItem)
            {
                case OFTypeSearchItem.Email:
                    return OFEmailHelper.Instance.GetEmailEmlFilename(item as OFEmailSearchObject); 
                case OFTypeSearchItem.Attachment:
                    return OFOutlookHelper.Instance.GetAttachmentTempFile(item as OFAttachmentContentSearchObject);
                case OFTypeSearchItem.Calendar:
                    return OFOutlookHelper.Instance.GetCalendarTempFileName(item);    
                case OFTypeSearchItem.File:
                case OFTypeSearchItem.Picture:
                    return forPreview ? GetNormalizeFilename(item, GetFileName(item.ItemUrl)) : GetFileName(item.ItemUrl);
                default:
                    return null;
            }
        }

        public static string GetFullFolderPath(OFBaseSearchObject item)
        {
            return OFOutlookHelper.Instance.GetFullFolderPath(item);
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

        private static string GetNormalizeFilename(OFBaseSearchObject item, string filename)
        {
            var ext = Path.GetExtension(filename);
            string normalFilename = string.Empty;
            switch (ext)
            {
                case ".pdf":
                    normalFilename = OFTempFileManager.Instance.GenerateTempFileNameWithCopy(item, filename);
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
