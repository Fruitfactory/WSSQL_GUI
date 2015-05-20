using System;

namespace OF.Core.Data.ElasticSearch.Response
{
    public enum OFRiverStatus
    {
        None,
        InitialIndexing,
        Busy,
        StandBy
    }

    public class OFRiverStatusInfo
    {
        public bool Success { get; set; }

        public OFRiverStatus Status { get; set; }

        public DateTime Lastupdated { get; set; }

    }
}