using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WSUI.Core.Data;
using System.Text.RegularExpressions;

namespace WSUI.Module.Service
{
    [ValueConversion(typeof(DateTime),typeof(string))]
    public class DateToStringConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = ((DateTime) value).ToLocalTime().ToShortDateString();
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }

    [ValueConversion(typeof(int),typeof(Visibility))]
    public class IntToVisibilityConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            int count = 0;
            if (value is int)
            {
                count = (int) value;
            }
            else if(value is string)
            {
                int.TryParse(value.ToString(), out count);
            }
            return count > 0 ? Visibility.Visible : Visibility.Collapsed ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }

    [ValueConversion(typeof(string),typeof(string))]
    public class CountToFormatStringConvert : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int count = 0;
            int.TryParse(value as string, out count);
            return count < 1 ? string.Empty : string.Format("[{0}]", count);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }

    [ValueConversion(typeof(int),typeof(string))]
    public class SizebytesToStringConverter: IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long size = (long) value / 1024;
            string result = string.Format("Size: {0} kb", size > 0 ? size : 1);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }

    [ValueConversion(typeof(BaseSearchObject), typeof(string))]
    public class ObjectToNameConverter : IValueConverter
    {
        private const string EmailPattern = @"\b[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,4}\b";
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = string.Empty;
            var contact = value as ContactSearchObject;
            if (contact != null)
            {
                
                var strEmail = IsEmail(contact.EmailAddress1) ?? IsEmail(contact.EmailAddress2) ?? IsEmail(contact.EmailAddress3);
                if(string.IsNullOrEmpty(contact.FirstName) || string.IsNullOrEmpty(contact.LastName))
                {
                    result = string.Format("{0} ({0})", strEmail);
                }
                else
                    result = string.Format( string.IsNullOrEmpty(strEmail) ? "{0} {1}":"{0} {1} ({2})", contact.FirstName, contact.LastName,strEmail);

                return result;
            }
            var email = value as EmailContactSearchObject;
            if (email != null)
            {
                if (string.IsNullOrEmpty(email.ContactName))
                {
                    result = string.Format("{0} ({0})", email.EMail);
                }
                else
                {
                    result = string.Format("{0} ({1})",email.ContactName, email.EMail);
                }
                
                return result;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        private string IsEmail(string email)
        {
            return !string.IsNullOrEmpty(email) && Regex.IsMatch(email, EmailPattern, RegexOptions.IgnoreCase) ? email : null;
        }
    }

    [ValueConversion(typeof(double),typeof(double))]
    public class ActualWidthToWidthConverter : IValueConverter
    {
        private readonly double ScrollbarWidth = 20;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                double val = (double) value;
                val = val - ScrollbarWidth;
                return val < 0 ? 0 : val;
            }
            if (parameter is Int32)
            {
                int param = (int) parameter;
                double val = (double) value;
                val = val - param;
                return val < 0 ? 0 : val;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(bool),typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public bool IsInvert { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return IsInvert ? value != null &&  !((bool) value) ? Visibility.Visible : Visibility.Collapsed :
                value != null &&  (bool) value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
