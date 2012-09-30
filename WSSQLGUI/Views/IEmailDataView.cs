using System;
using System.Collections.Generic;
using System.Text;
using WSSQLGUI.Core;
using WSSQLGUI.Models;

namespace WSSQLGUI.Views
{
	internal interface IEmailDataView : IDataView
	{
		EmailSearchData CurrentEmailItem
		{
			get;
		}
	}
}
