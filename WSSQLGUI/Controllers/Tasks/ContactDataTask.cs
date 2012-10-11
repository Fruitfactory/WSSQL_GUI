using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCSharp.Core.Tasks;
using MVCSharp.Core.Configuration.Tasks;

namespace WSSQLGUI.Controllers.Tasks
{
    internal class ContactDataTask : TaskBase
    {
        [IPoint(typeof(ContactDataController))]
        public const string ContactDataView = "ContactDataView";

        public override void OnStart(object param)
        {
            Navigator.ActivateView(ContactDataView);
        }
    }
}
