using OF.Core.Core.Payload;
using OF.Core.Interfaces;

namespace OF.Infrastructure.Payloads
{
    public class OFSearchObjectPayload : BasePayload<ISearchObject>
    {
        public OFSearchObjectPayload(ISearchObject arg) : base(arg)
        {
        }
    }
}