using System;
using MVCSharp.Core.CommandManager;
using WSSQLGUI.Services;

namespace WSSQLGUI.Core
{
    internal interface IBaseSettingsController
    {
        ICommand SearchCommand { get; }
        bool IsLoading { get; set; }
        event EventHandler Search;
        event EventHandler<EventArgs<bool>> Error;
    }
}