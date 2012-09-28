using System;
using System.Collections.Generic;
using System.Text;
using WSSQLGUI.Core;

namespace WSSQLGUI.Views
{
	public interface IEmailDataView : IDataView
	{
		EmailSearchData CurrentEmailItem
		{
			get;
			set;
		}
	}
}
