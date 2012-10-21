using System;
using System.Collections.Generic;
using WSSQLGUI.Services;

namespace WSSQLGUI.Controllers
{
    internal interface IContactSettingsController
    {
        event EventHandler<EventArgs<List<string>>> Suggest;
        void StartSuggesting(string text);
    }
}