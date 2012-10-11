using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCSharp.Core.Tasks;
using MVCSharp.Core.Configuration.Tasks;

namespace WSSQLGUI.Controllers.Tasks
{
    internal class EmailDataTask : TaskBase
    {

        [IPoint(typeof(EmailDataController))]
        public const string EmailDataView = "EmailDataView";


        public override void OnStart(object param)
        {
            Navigator.ActivateView(EmailDataView);
        }
    }
}
