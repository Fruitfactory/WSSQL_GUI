using System;
using System.Collections.Generic;
using WSUI.Core.Core;

namespace WSUI.Infrastructure.Models
{

   
    public class EmailData
    {
        private List<EmailSearchData> _data = new List<EmailSearchData>();

        public List<EmailSearchData> Items 
        { 
            get 
            {
                return _data; 
            } 
        }
    }

    
	public class EmailSearchData : BaseSearchData
	{
        public string Subject
        {
            get;
            set;
        }

        public string Recepient
        {
            get;
            set;
        }

        public DateTime Date
        {
            get;
            set;
        }

        public List<string> Attachments
        {
            get;
            set;
        }

        public string ConversationIndex { get; set; }

        public string ConversationId { get; set; }

        public string From { get; set; }

        public string LastId { get; set; }

    }

    public class EmailSearchDataComparer : IEqualityComparer<EmailSearchData>
    {
        public bool Equals(EmailSearchData x, EmailSearchData y)
        {
            if (x.ConversationIndex == y.ConversationIndex)
                return true;
            return false;
        }

        public int GetHashCode(EmailSearchData obj)
        {
            return obj.ConversationIndex.GetHashCode();
        }
    }

}
