﻿using System.IO.Packaging;
using Newtonsoft.Json;
using WSUI.Core.Enums;

namespace WSUI.Core.Data.ElasticSearch.Response
{
    public class WSUIStatusItem
    {
        
        public string Name { get; set; }

        public int Count { get; set; }

        public int Processing { get; set; }

        public PstReaderStatus Status { get; set; }
    }
}