using OF.Core.Core.Payload;
using OF.Core.Interfaces;

namespace OF.Infrastructure.Payloads
{
    public class SearchObjectPayload : BasePayload<ISearchObject>
    {
        public SearchObjectPayload(ISearchObject arg) : base(arg)
        {
        }
    }
}