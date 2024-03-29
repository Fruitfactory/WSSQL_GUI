﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSSQLGUI.Services.Enums;

namespace WSSQLGUI.Models
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
