using System;
using System.Collections.Generic;
using System.Text;
using WSSQLGUI.Core;
using WSSQLGUI.Models;
using WSSQLGUI.Views;

namespace WSSQLGUI.Controllers
{
    internal class EmailDataController : BaseDataController
    {

        public override void SetData(BaseSearchData item)
        {
            if (item == null || !(item is EmailSearchData))
                return;
            EmailSearchData email = item as EmailSearchData;
            var value = new object[] { email.Recepient, email.Subject, email.Date };
            (View as IDataView).SetData(value, email);

        }
    }
}
