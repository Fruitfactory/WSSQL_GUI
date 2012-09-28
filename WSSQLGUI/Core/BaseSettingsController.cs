using System;
using System.Collections.Generic;
using System.Text;
using MVCSharp.Core;
using MVCSharp.Core.CommandManager;
using WSSQLGUI.Services;


namespace WSSQLGUI.Core
{
	internal abstract class BaseSettingsController : ControllerBase
	{
        protected DelegateCommand _searchCommand;
        protected string _nameButton = "Search";

		public virtual string ErrorProvider(string textForAnalyz)
		{
            return String.Empty;
		}

		public ICommand SearchCommand
		{
			get
			{
                if (_searchCommand == null)
                    _searchCommand = new DelegateCommand(_nameButton, OnCanSearch, OnSearch);
                return _searchCommand;
			}
		}

		public event EventHandler Search;
        public event EventHandler<EventArgs<bool>> Error;

        protected virtual bool OnCanSearch()
        {
            return true;
        }

        protected virtual void OnSearch()
        {
            EventHandler temp = Search;
            if (temp != null)
            {
                temp(this, new EventArgs());
            }
        }

        protected void OnError(bool value)
        {
            EventHandler<EventArgs<bool>> temp = Error;
            if (temp != null)
            {
                temp(null, new EventArgs<bool>(value));
            }
        }

	}
}
