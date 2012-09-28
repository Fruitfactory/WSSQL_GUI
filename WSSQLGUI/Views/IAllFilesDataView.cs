using System;
using System.Collections.Generic;
using System.Text;
using WSSQLGUI.Core;

namespace WSSQLGUI.Views
{
	public interface IAllFilesDataView : IDataView
	{
		BaseSearchData CurrentFilelItem
		{
			get;
		}

        void SetData(object[] data, BaseSearchData addData)
        {}


	}
}
