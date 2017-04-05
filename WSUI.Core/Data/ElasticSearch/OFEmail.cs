using System;
using Nest;

namespace OF.Core.Data.ElasticSearch
{
    [ElasticsearchType(Name = "email")]
    public class OFEmail : OFElasticSearchBaseEntity
    {
        public OFEmail()
        {
            
        }
        public string ItemName { get; set; }

        public string ItemUrl { get; set; }

        public string Folder { get; set; }

        public string Foldermessagestoreidpart { get; set; }

        public string Storagename { get; set; }

        public DateTime? Datecreated { get; set; }

        public DateTime? Datereceived { get; set; }

        public long Size { get; set; }

        public string Conversationid { get; set; }

        public string Conversationindex { get; set; }

        public string Outlookconversationid { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }

        public string Htmlcontent { get; set; }

        public string Analyzedcontent { get; set; }

        public string Hasattachments { get; set; }

        public string Fromname { get; set; }

        public string Fromaddress { get; set; }

        public string Storeid { get; set; }

        public OFRecipient[] To { get; set; }

        public OFRecipient[] Cc { get; set; }

        public OFRecipient[] Bcc { get; set; }

        public OFAttachment[] Attachments { get; set; }

        public override string ToString()
        {
            return string.Format("Subject<{0}> From<{1} {2}>", Subject, Fromname, Fromaddress);
        }
    }
}