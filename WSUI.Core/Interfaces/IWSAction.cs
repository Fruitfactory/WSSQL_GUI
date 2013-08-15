using System;
using WSUI.Core.Enums;

namespace WSUI.Core.Interfaces
{
    public interface IWSAction
    {
        WSActionType Action { get; }
        Object Data { get; }
    }
}