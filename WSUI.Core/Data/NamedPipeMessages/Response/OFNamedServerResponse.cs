using OF.Core.Enums;

namespace OF.Core.Data.NamedPipeMessages.Response
{
    public class OFNamedServerResponse
    {
        public ofServerResponseStatus Status { get; set; }

        public object Body { get; set; }

        public string Message { get; set; }
    }
}