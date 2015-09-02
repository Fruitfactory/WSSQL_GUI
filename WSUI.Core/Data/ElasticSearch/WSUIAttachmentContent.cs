using Nest;

namespace OF.Core.Data.ElasticSearch
{
    [ElasticType(Name = "attachment")]
    public class OFAttachmentContent : OFElasticSearchBaseEntity
    {
        public OFAttachmentContent()
        {
            
        }

        public string Filename { get; set; }

        public string Path { get; set; }

        public long Size { get; set; }

        public string Mimetag { get; set; }

        public string Content { get; set; }

        public string Analyzedcontent { get; set; }

        public string Emailid { get; set; }
    }
}