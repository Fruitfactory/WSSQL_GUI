using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSUI.Infrastructure.Core;

namespace WSUI.Infrastructure.Models
{
    public class ContactSearchData : BaseSearchData
    {

        private readonly List<string> _emailList = new List<string>();

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
       
    }

}
