using System;
using System.Collections.Generic;
using System.Text;
using WSSQLGUI.Core;

namespace WSSQLGUI.Views
{
	public interface IEmailSettingsView : ISettingsView
	{
		string Folder
		{
			get;
			set;
		}
	}
}
