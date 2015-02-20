using System;
using Nest;

namespace WSUI.Core.Data.ElasticSearch
{
    [ElasticType(Name = "email")]
    public class WSUIEmail : WSUIElasticSearchBaseEntity
    {
        public string ItemName { get; set; }

        public string ItemUrl { get; set; }

        public string Folder { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateReceived { get; set; }

        public long Size { get; set; }

        public string ConversationId { get; set; }

        public string ConversationIndex { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }

        public string HasAttachments { get; set; }

        public string FromName { get; set; }

        public string FromAddress { get; set; }

        public WSUIRecipient[] To { get; set; }

        public WSUIRecipient[] Cc { get; set; }

        public WSUIRecipient[] Bcc { get; set; }

        public WSUIAttachment[] Attachments { get; set; }

        public override string ToString()
        {
            return string.Format("Subject<{0}> From<{1} {2}>", Subject, FromName, FromAddress);
        }
    }
}