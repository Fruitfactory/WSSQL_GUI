using System;
using System.Collections.Generic;
using System.Text;
using WSUI.Infrastructure.Service;
using WSUI.Infrastructure.Services;


namespace WSUI.Infrastructure.Core
{
	internal interface IKindItem
	{
		string Name
		{
			get;
		}

        string SearchString { get; set; }

	    string Prefix { get; }

		object SettingsView
		{
			get;
		}

        object DataView
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

        void ConnectWithSettingsView(object settingsView);
        void ConnectWithDataView(object dataView);
	    void OnInit();
	}
}
