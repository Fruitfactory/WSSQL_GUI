﻿using System;
using Nest;

namespace WSUI.Core.Data.ElasticSearch
{
    [ElasticType(Name = "email")]
    public class WSUIEmail : WSUIElasticSearchBaseEntity
    {
        public string ItemName { get; set; }

        public string ItemUrl { get; set; }

        public string Folder { get; set; }

        public string Foldermessagestoreidpart { get; set; }

        public string Storagename { get; set; }

        public DateTime Datecreated { get; set; }

        public DateTime Datereceived { get; set; }

        public long Size { get; set; }

        public string Conversationid { get; set; }

        public string Conversationindex { get; set; }

        public string Outlookconversationid { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }

        public string Htmlcontent { get; set; }

        public string Analyzedcontent { get; set; }

        public string Hasattachments { get; set; }

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