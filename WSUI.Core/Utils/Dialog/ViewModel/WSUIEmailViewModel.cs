using System;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WSUI.Core.Utils.Dialog.Interfaces;

namespace WSUI.Core.Utils.Dialog.ViewModel
{
    public class WSUIEmailViewModel : WSUIBaseDialogViewModel,IWSUIEmailViewModel
    {

        #region [needs]

        private string _email1 = string.Empty;
        private string _email2 = string.Empty;
        private const string EmailPattern = @"\b[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,4}\b";
        private const string TheEmailShouldnTBeEmpty = "The email shouldn't be empty.";
        private const string PleaseEnterAValidEmailAddress = "Please enter a valid email address.";

        private bool _isValid = false;
        private bool _isEqual = false;

        SolidColorBrush _validBrush = new SolidColorBrush(Colors.SpringGreen);
        SolidColorBrush _errorBrush = new SolidColorBrush(Colors.OrangeRed);

        #endregion

        public WSUIEmailViewModel(IWSUIView view) 
            : base(view)
        {
        }

        protected override string Validate(string columnName)
        {
            string result = string.Empty;
            
            switch (columnName)
            {
                case "Email1":
                    if (string.IsNullOrEmpty(Email1))
                        result = TheEmailShouldnTBeEmpty;
                    if (!IsEmail(Email1))
                        result = PleaseEnterAValidEmailAddress;
                    break;
                case "Email2":
                    if (string.IsNullOrEmpty(Email2))
                        result = TheEmailShouldnTBeEmpty;
                    if (!IsEmail(Email2))
                        result = PleaseEnterAValidEmailAddress;
                    break;
            }
            _isValid = string.IsNullOrEmpty(result);
            return result;
        }

        public string Email1
        {
            get { return _email1; }
            set { _email1 = value; CheckIfEmailsEqual();}
        }
    
        public string Email2 
        {
            get { return _email2; }
            set { _email2 = value; CheckIfEmailsEqual();}
        }

        public SolidColorBrush ValidBrush { get; private set; }


        protected override bool CanOkExecute(object arg)
        {
            return _isValid && _isEqual;
        }

        protected override string FormatTitle()
        {
            return "Emails";
        }

        #region [private]

        private void CheckIfEmailsEqual()
        {
            _isEqual = Email1 == Email2;
            ValidBrush = _isEqual ? _validBrush : _errorBrush;
            OnPropertyChanged(() => ValidBrush);
        }

        private bool IsEmail(string email)
        {
            return !string.IsNullOrEmpty(email) && Regex.IsMatch(email, EmailPattern, RegexOptions.IgnoreCase);
        }



        #endregion
    }
}