using Newtonsoft.Json;

namespace OF.Core.Data.Settings.ControllerSettings
{
    public class OFEveryHourPeriodSettings
    {
        [JsonProperty("hour_period")]
        public int HourPeriod { get; set; }
    }
}