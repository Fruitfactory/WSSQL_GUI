using System;
using System.Collections.Generic;
using System.Text;
using WSSQLGUI.Core;
using System.Text.RegularExpressions;

namespace WSSQLGUI.Controllers
{
	public class AllFilesSettingsController : BaseSettingsController
	{
        private bool _isError = false;

        public override string ErrorProvider(string textForAnalyz)
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


        protected override bool OnCanSearch()
        {
            return !_isError;
        }

	}
}
