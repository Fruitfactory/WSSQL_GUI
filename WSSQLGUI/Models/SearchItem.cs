using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSSQLGUI.Models
{
    class SearchItem
    {
        public SearchItem()
        {}

        public string Name { get; set; }
        public string FileName { get; set; }
    }

    class ListSearchItems : List<SearchItem>
    {}
}
