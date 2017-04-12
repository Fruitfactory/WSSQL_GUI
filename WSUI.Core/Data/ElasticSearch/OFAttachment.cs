using Nest;

namespace OF.Core.Data.ElasticSearch
{
    
    public class OFAttachment : OFElasticSearchBaseEntity
    {
        public string Filename { get; set; }

        public string Path { get; set; }

        public long Size { get; set; }

        public string Mimetag { get; set; }
    }
}