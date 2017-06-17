using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace OF.Core.Data.Settings.ControllerSettings
{
    public class OFNightIdleSettings
    {
        [JsonProperty("idle_time")]
        public int IdleTime { get; set; }
    }
}
