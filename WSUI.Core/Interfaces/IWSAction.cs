using System;
using OF.Core.Enums;

namespace OF.Core.Interfaces
{
    public interface IWSAction
    {
        OFActionType Action { get; }

        Object Data { get; }
    }
}