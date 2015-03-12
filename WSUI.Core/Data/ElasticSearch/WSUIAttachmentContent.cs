using Nest;

namespace WSUI.Core.Data.ElasticSearch
{
    [ElasticType(Name = "attachment")]
    public class WSUIAttachmentContent : WSUIElasticSearchBaseEntity
    {
        public string Filename { get; set; }

        public string Path { get; set; }

        public long Size { get; set; }

        public string MimeTag { get; set; }

        public string Content { get; set; }

        public string EmailId { get; set; }
    }
}