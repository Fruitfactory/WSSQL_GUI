using System;
using System.Collections.Generic;
using System.Text;
using WSSQLGUI.Core;


namespace WSSQLGUI.Models
{

   
    internal class EmailData
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

    
	internal class EmailSearchData : BaseSearchData
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
       
    }

    internal class EmailSearchDataComparer : IEqualityComparer<EmailSearchData>
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
