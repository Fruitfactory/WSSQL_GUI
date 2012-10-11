using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCSharp.Core.Configuration.Tasks;
using MVCSharp.Core.Tasks;

namespace WSSQLGUI.Controllers.Tasks
{
    internal class MainTask : TaskBase
    {
        [InteractionPoint(typeof(SearchController))]
        public const string Search = "Search";

        public override void OnStart(object param)
        {
            Navigator.NavigateDirectly(Search);
        }

    }
}
