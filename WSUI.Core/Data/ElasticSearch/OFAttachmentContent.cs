using System;
using Nest;
using Newtonsoft.Json;
using OF.Core.Core.Attributes;

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

        [OFIgnore]
        public string Content { get; set; }

        [OFIgnore]
        public string Analyzedcontent { get; set; }

        public string Emailid { get; set; }

        public string Outlookemailid { get; set; }

        public DateTime? Datecreated { get; set; }
    }
}