using System;
using WSUI.Infrastructure.Services;

namespace WSUI.Infrastructure.Core
{
    internal interface IBaseSettingsController
    {
//        ICommand SearchCommand { get; }
        bool IsLoading { get; set; }
        event EventHandler Search;
        event EventHandler<EventArgs<bool>> Error;
    }
}