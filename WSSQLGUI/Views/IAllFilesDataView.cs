using System;
using System.Collections.Generic;
using System.Text;
using WSSQLGUI.Core;

namespace WSSQLGUI.Views
{
	internal interface IAllFilesDataView : IDataView
	{
		BaseSearchData CurrentFilelItem
		{
			get;
		}
	}
}
