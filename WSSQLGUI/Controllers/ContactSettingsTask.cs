using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCSharp.Core.Tasks;
using MVCSharp.Core.Configuration.Tasks;

namespace WSSQLGUI.Controllers
{
    internal class ContactSettingsTask : TaskBase
    {
        [IPoint(typeof(ContactSettingsController))]
        public const string ContactSettingsView = "ContactSettingsView";

        public override void OnStart(object param)
        {
            Navigator.ActivateView(ContactSettingsView);
        }

    }
}
