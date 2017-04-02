using Newtonsoft.Json;

namespace OF.Core.Data.NamedPipeMessages
{
    public class OFIsForcedMessage
    {
        [JsonProperty("is_forced")]
        public bool IsForced { get; set; }
    }
}