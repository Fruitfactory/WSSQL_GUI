using OF.Core.Enums;
using OF.Core.Interfaces;

namespace OF.Core.Data
{
    public class WSAction : IWSAction
    {
        public WSAction(WSActionType type, object data)
        {
            Action = type;
            Data = data;
        }

        public WSActionType Action { get; private set; }

        public object Data { get; private set; }
    }
}