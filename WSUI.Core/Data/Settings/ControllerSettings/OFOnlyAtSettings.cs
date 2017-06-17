using Newtonsoft.Json;

namespace OF.Core.Data.Settings.ControllerSettings
{
    public class OFOnlyAtSettings
    {
        [JsonProperty("hour_only_at")]
        public int HourOnlyAt { get; set; }

        [JsonProperty("hour_type")]
        public string HourType { get; set; }

    }
}