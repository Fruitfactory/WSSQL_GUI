using System;
using WSUI.Services;

namespace WSUI.Core
{
    internal interface IBaseSettingsController
    {
//        ICommand SearchCommand { get; }
        bool IsLoading { get; set; }
        event EventHandler Search;
        event EventHandler<EventArgs<bool>> Error;
    }
}