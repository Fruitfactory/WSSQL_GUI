using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCSharp.Core.Tasks;
using MVCSharp.Core.Configuration.Tasks;

namespace WSSQLGUI.Controllers
{
    internal class EmailSettingsTask : TaskBase
    {
        [IPoint(typeof(EmailSettingsController))]
        public const string EmailSettingsView = "EmailSettingsView";

        public override void OnStart(object param)
        {
            Navigator.ActivateView(EmailSettingsView);
        }
    }
}
