using System;
using System.Collections.Generic;
using System.Text;
using WSSQLGUI.Core;
using System.Text.RegularExpressions;

namespace WSSQLGUI.Controllers
{
	internal class AllFilesSettingsController : BaseSettingsController
	{
        

        protected override bool OnCanSearch()
        {
            return !_isError && !_isLoading;
        }

	}
}
