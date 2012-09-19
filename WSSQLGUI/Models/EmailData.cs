using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSSQLGUI.Models
{
    enum TypeRecord
    {
        Group,
        Item
    }


    interface IEmailData
    {
        TypeRecord Type { get; set; }
    }


    class EmailData:IEmailData
    {
        private List<EmailItem> _items = new List<EmailItem>();

        public TypeRecord Type
        {
            get;
            set;
        }

        public List<EmailItem> Items { get { return _items; } }
    }

    class EmailItem:IEmailData
    {
        public TypeRecord Type
        {
            get;
            set;
        }
        public string Subject { get; set; }
        public string ItemName { get; set; }
        public string ItemUrl { get; set; }
        public string Recepient { get; set; }
        public DateTime Date { get; set; }


    }
}
