using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSSQLGUI.Core;

namespace WSSQLGUI.Models
{
    internal class ContactSearchData : BaseSearchData
    {
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

        public string EmailAddress
        {
            get;
            set;
        }

        public string EmailAddress2
        {
            get;
            set;
        }

        public string EmailAddress3
        {
            get;
            set;
        }

        public string Foto { get; set; }
       
    }
}
