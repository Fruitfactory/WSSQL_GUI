namespace WSUI.Core.Data
{
    public class AttachmentContentSearchObject : AbstractSearchObject
    {
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

        public long Size
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

    }
}