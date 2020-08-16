﻿namespace OF.Core.Data.ElasticSearch.Response
{
    public class OFHitsRaw<T> where T : class
    {
        public OFHitsTotal total { get; set; }

        public object max_score { get; set; }

        public OFHitRaw<T>[] hits { get; set; }
    }
}