using System;
using System.Collections.Generic;
using System.Text;

namespace WSUI.Infrastructure.Core
{
	internal interface ISettingsView
	{
		string SearchCriteria
		{
			get;
			set;
		}
	}
}
