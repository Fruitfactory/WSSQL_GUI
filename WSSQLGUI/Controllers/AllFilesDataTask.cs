using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCSharp.Core.Tasks;
using MVCSharp.Core.Configuration.Tasks;

namespace WSSQLGUI.Controllers
{
    internal class AllFilesDataTask : TaskBase
    {
      
        [IPoint(typeof(AllFilesDataController))]
        public const string AllFilesDataView = "AllFilesDataView";

        public override void OnStart(object param)
        {
            Navigator.ActivateView(AllFilesDataView);
        }
    }
}
