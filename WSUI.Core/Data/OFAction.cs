using OF.Core.Enums;
using OF.Core.Interfaces;

namespace OF.Core.Data
{
    public class OFAction : IWSAction
    {
        public OFAction(OFActionType type, object data)
        {
            Action = type;
            Data = data;
        }

        public OFActionType Action { get; private set; }

        public object Data { get; private set; }
    }
}