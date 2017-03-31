using System;
using System.Collections.Generic;
using Microsoft.Win32;
using Newtonsoft.Json;
using OF.Core.Enums;

namespace OF.Core.Data.ElasticSearch
{
    public class OFRiverMeta
    {
        public OFRiverMeta()
        {   
        }

        public OFRiverMeta(string indexName )
        {
            Type = "pst";
            Pst = new OFPstMeta("1h","3m","3m",indexName);
        }
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("pst")]
        public OFPstMeta Pst { get; set; }

        [JsonProperty("lastdate")]
        public DateTime? LastDate { get; set; }

    }

    public class OFPstMeta
    {
        public OFPstMeta()
        {
        }

        public OFPstMeta(string updateRate, string onlibeTime, string idleTime, string indexName )
        {
            UpdateRate = updateRate;
            OnlineTime = onlibeTime;
            IdleTime = idleTime;
            IndexName = indexName;
            Schedule = new OFScheduleMeta();
        }

        [JsonProperty("update_rate")]
        public string UpdateRate { get; set; }
        [JsonProperty("online_time")]
        public string OnlineTime { get; set; }
        [JsonProperty("idle_time")]
        public string IdleTime { get; set; }
        
        [JsonProperty("index_name")]
        public string IndexName { get; set; }

        [JsonProperty("schedule")]
        public OFScheduleMeta Schedule { get; set; }

    }


    public class OFScheduleMeta
    {
        public OFScheduleMeta()
        {

#if DEBUG
            // user tracking

            ScheduleType = OFRiverSchedule.EveryNightOrIdle;
            var idlesettings = new { idle_time = 60 };
            var set = JsonConvert.SerializeObject(idlesettings);
            Settings = set;

            //period

            //ScheduleType = OFRiverSchedule.EveryHours;
            //var idlesettings = new { hour_period = 1 };
            //var set = JsonConvert.SerializeObject(idlesettings);
            //Settings = set;
#else
            // user tracking

            ScheduleType = OFRiverSchedule.EveryNightOrIdle;
            var idlesettings = new { idle_time = 300 };
            var set = JsonConvert.SerializeObject(idlesettings);
            Settings = set;

            //period

            //ScheduleType = RiverSchedule.EveryHours;
            //var idlesettings = new { hour_period = 1 };
            //var set = JsonConvert.SerializeObject(idlesettings);
            //Settings = set;

#endif

        }

        [JsonProperty("schedule_type")]
        public OFRiverSchedule ScheduleType { get; set; }

        [JsonProperty("settings")]
        public string Settings { get; set; }

    }

}