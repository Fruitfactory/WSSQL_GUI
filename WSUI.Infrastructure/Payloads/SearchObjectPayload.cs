using WSUI.Core.Core.Payload;
using WSUI.Core.Interfaces;

namespace WSUI.Infrastructure.Payloads
{
    public class SearchObjectPayload : BasePayload<ISearchObject>
    {
        public SearchObjectPayload(ISearchObject arg) : base(arg)
        {
        }
    }
}