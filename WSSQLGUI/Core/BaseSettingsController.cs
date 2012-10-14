using System;
using System.Collections.Generic;
using System.Text;
using MVCSharp.Core;
using MVCSharp.Core.CommandManager;
using WSSQLGUI.Services;
using MVCSharp.Core.Views;
using WSSQLGUI.Controllers;
using System.Text.RegularExpressions;


namespace WSSQLGUI.Core
{
	internal abstract class BaseSettingsController : ControllerBase
	{
        protected DelegateCommand _searchCommand;
        protected string _nameButton = "Search";
        protected bool _isError = false;
	    protected bool _isLoading = false;

		public virtual string ErrorProvider(string textForAnalyz)
		{
            string pattern = @"\bselect\s.*from\b";
            var message = string.Empty;
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = reg.Match(textForAnalyz);
            if (m.Success)
            {
                _isError = true;
                message = "You have written wrong Search Criteria";
            }
            else
            {
                _isError = false;
            }
            OnError(_isError);
            return message;
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

	    public bool IsLoading
	    {
            get { return _isLoading; }
            set { _isLoading = value; }
	    }

		public event EventHandler Search;
        public event EventHandler<EventArgs<bool>> Error;

        protected virtual bool OnCanSearch()
        {
            return !_isLoading;
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
