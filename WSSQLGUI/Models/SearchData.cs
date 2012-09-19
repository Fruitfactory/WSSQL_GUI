using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSSQLGUI.Models
{
    class SearchData
    {
        public string ItemName { get; set; }
        public string ItemUrl { get; set; }
        public bool IsAttachment { get; set; }
        public string ConversationID { get; set;}
        public string Date { get; set; }
    }
}
