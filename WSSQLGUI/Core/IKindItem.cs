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

		IView SettingsView
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
        event EventHandler<EventArgs<BaseSearchData>> CurrentItemChanged;

		int ID
		{
			get;
		}

        void ConnectWithSettingsView(IView settingsView);
        void ConnectWithDataView(IView dataView);

	}
}
