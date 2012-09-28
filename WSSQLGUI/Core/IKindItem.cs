using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using WSSQLGUI.Services;
using MVCSharp.Core.Views;

namespace WSSQLGUI.Core
{
	internal interface IKindItem
	{
		string Name
		{
			get;
		}

		IView SettingsVIew
		{
			get;
		}

        IView DataView
		{
			get;
		}

		event EventHandler Start;

		event EventHandler<EventArgs<bool>> Complete;
        event EventHandler<EventArgs<bool>> Error;

		int ID
		{
			get;
		}
	}
}
