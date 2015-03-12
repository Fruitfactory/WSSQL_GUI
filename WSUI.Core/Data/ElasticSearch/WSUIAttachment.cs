using Nest;

namespace WSUI.Core.Data.ElasticSearch
{
    
    public class WSUIAttachment : WSUIElasticSearchBaseEntity
    {
        public string FileName { get; set; }

        public string Path { get; set; }

        public long Size { get; set; }

        public string MimeTag { get; set; }
    }
}