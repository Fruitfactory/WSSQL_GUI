using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSSQLGUI.Core;
using WSSQLGUI.Models;

namespace WSSQLGUI.Views
{
    internal interface IContactDataView : IDataView
    {
        void SetContact(BaseSearchData contact);
        ContactSearchData CurrentContact { get; }
    
    }
}
