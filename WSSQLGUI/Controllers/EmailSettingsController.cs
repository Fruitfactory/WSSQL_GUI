using System;
using System.Collections.Generic;
using System.Text;
using WSSQLGUI.Core;
using WSSQLGUI.Services.Helpers;

namespace WSSQLGUI.Controllers
{
	internal class EmailSettingsController : BaseSettingsController, IEmailSettings
	{
		public List<string> GetFolders()
		{
            var list = OutlookHelper.Instance.GetFolderList();
            return list;
		}
	}
}
