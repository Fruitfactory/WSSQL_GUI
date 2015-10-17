using System.Runtime.InteropServices;
using OF.Core.Enums;

namespace OF.Core.Data
{
    public class OFAttachmentContentSearchObject : OFBaseSearchObject
    {

        public OFAttachmentContentSearchObject()
        {
            TypeItem = OFTypeSearchItem.Attachment;
        }

        public override string ItemName
        {
            get { return Filename; }
            set { Filename = value; }
        }

        public override string ItemUrl
        {
            get { return Filename; }
            set { Filename = value; }
        }

        public override string ItemNameDisplay
        {
            get { return Filename; }
            set { Filename = value; }
        }

        public string Filename
        {
            get { return Get(() => Filename); }
            set { Set(() => Filename, value); }
        }

        public string Path
        {
            get { return Get(() => Path); }
            set { Set(() => Path, value); }
        }

        public new long Size
        {
            get { return Get(() => Size); }
            set { Set(() => Size, value); }
        }

        public string MimeTag
        {
            get { return Get(() => MimeTag); }
            set { Set(() => MimeTag, value); }
        }

        public string Content
        {
            get { return Get(() => Content); }
            set { Set(() => Content, value); }
        }

        public string EmailId
        {
            get { return Get(() => EmailId); }
            set { Set(() => EmailId, value); }
        }

        public string Outlookemailid
        {
            get { return Get(() => Outlookemailid); }
            set { Set(() => Outlookemailid, value); }
        }

    }
}