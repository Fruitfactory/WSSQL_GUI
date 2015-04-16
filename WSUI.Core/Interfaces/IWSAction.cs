using System;
using OF.Core.Enums;

namespace OF.Core.Interfaces
{
    public interface IWSAction
    {
        WSActionType Action { get; }

        Object Data { get; }
    }
}