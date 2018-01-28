using System;
using OF.Core.Core.Payload;

namespace OF.Infrastructure.Payloads
{
    public class OFSuggestedEmailPayload : BasePayload<Tuple<IntPtr, string>>
    {
        public OFSuggestedEmailPayload(Tuple<IntPtr, string> arg) : base(arg)
        {
        }
    }
}