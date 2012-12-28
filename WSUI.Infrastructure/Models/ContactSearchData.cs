using System.Collections.Generic;
using WSUI.Infrastructure.Core;

namespace WSUI.Infrastructure.Models
{
    public class ContactSearchData : BaseSearchData
    {

        private readonly List<string> _emailList = new List<string>();
        private  readonly List<EmailSearchData> _emails = new List<EmailSearchData>(); 
        public string FirstName
        {
            get;
            set;
        }

        public string LastName
        {
            get;
            set;
        }

        public List<string> EmailList
        {
            get { return _emailList; }
        } 
        
        public string Foto { get; set; }
       
        public List<EmailSearchData> Emails
        {
            get { return _emails; }
        }

    }

}
