namespace OF.Core.Data.ElasticSearch.Response
{
    public class OFShardsRaw
    {
        public int total { get; set; }

        public int successful { get; set; }

        public int skipped { get; set; }

        public int failed { get; set; }
    }
}