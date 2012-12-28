using System;
using System.Collections.Generic;
using WSUI.Infrastructure.Service.Enums;

namespace WSUI.Infrastructure.Models
{
    class SearchItem
    {
        public SearchItem()
        {}
        public string Subject { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Recepient { get; set; }
        public DateTime Date { get; set; }

        public TypeSearchItem Type
        {
            get;
            set;
        }

        public bool IsAttachment
        {
            get;
            set;
        }

        public Guid ID
        {
            get;
            set;
        }

    }

    class ListSearchItems : List<SearchItem>
    {}
}
