using Nest;

namespace OF.Core.Data.ElasticSearch
{
    
    public class OFAttachment : OFElasticSearchBaseEntity
    {
        public string FileName { get; set; }

        public string Path { get; set; }

        public long Size { get; set; }

        public string MimeTag { get; set; }
    }
}