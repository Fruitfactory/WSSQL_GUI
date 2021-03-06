using System;
using System.Collections.Generic;
using System.Text;
using WSSQLGUI.Services.Enums;

namespace WSSQLGUI.Core
{
	internal class BaseSearchData
	{
        public string Name
        {
            get;
            set;
        }

        public string Path
        {
            get;
            set;
        }

        public TypeSearchItem Type
        {
            get;
            set;
        }

        public Guid ID
        {
            get;
            set;
        }
	}
}
