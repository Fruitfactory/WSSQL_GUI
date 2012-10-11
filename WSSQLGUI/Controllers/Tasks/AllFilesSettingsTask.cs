using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCSharp.Core.Tasks;
using MVCSharp.Core.Configuration.Tasks;

namespace WSSQLGUI.Controllers.Tasks
{
    internal class AllFilesSettingsTask : TaskBase
    {
        [IPoint(typeof(AllFilesSettingsController))]
        public const string AllFilesSettingsView = "AllFilesSettingsView";

        public override void OnStart(object param)
        {
            Navigator.ActivateView(AllFilesSettingsView);
        }
    }
}
