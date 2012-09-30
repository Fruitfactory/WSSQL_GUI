using System;
using System.Collections.Generic;
using System.Text;
using WSSQLGUI.Core;
using WSSQLGUI.Views;


namespace WSSQLGUI.Controllers
{
	internal class AllFilesDataController : BaseDataController
	{

        public override void SetData(BaseSearchData item)
        {
            if(item ==  null)
                return;
            var value = new object[] { item.Name, item.Path };
            (View as IAllFilesDataView).SetData(value, item);
        }

	}
}
