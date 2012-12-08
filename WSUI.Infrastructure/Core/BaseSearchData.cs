using System;
using System.Collections.Generic;
using System.Text;
using WSUI.Infrastructure.Service.Enums;

namespace WSUI.Infrastructure.Core
{
	public class BaseSearchData
	{
        public string Name
        {
            get;
            set;
        }

        public string Display { get; set; }

        public string Path
        {
            get;
            set;
        }

        public DateTime DateModified { get; set; }

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

        public object Tag { get; set; }

        public string Count { get; set; }
	}
}
