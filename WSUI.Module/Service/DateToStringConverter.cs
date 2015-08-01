using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using OF.Core.Data;
using System.Text.RegularExpressions;
using System.Windows.Media;
using OF.Core.Data.ElasticSearch.Response;
using OF.Core.Extensions;

namespace OF.Module.Service
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateToStringConverter : IValueConverter
    {

        public bool WithTime { get; set; }

        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = !WithTime ? ((DateTime)value).ToLocalTime().ToShortDateString() : ((DateTime)value).ToLocalTime().ToString("g");
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }

    [ValueConversion(typeof(int), typeof(Visibility))]
    public class IntToVisibilityConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            int count = 0;
            if (value is int)
            {
                count = (int)value;
            }
            else if (value is string)
            {
                int.TryParse(value.ToString(), out count);
            }
            return count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }

    [ValueConversion(typeof(string), typeof(string))]
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

    [ValueConversion(typeof(int), typeof(string))]
    public class SizebytesToStringConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long size = (long)value / 1024;
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
                if (string.IsNullOrEmpty(contact.FirstName) || string.IsNullOrEmpty(contact.LastName))
                {
                    result = string.Format("{0} ({0})", strEmail);
                }
                else
                    result = string.Format(string.IsNullOrEmpty(strEmail) ? "{0} {1}" : "{0} {1} ({2})", contact.FirstName, contact.LastName, strEmail);

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
                    result = string.Format("{0} ({1})", email.ContactName, email.EMail);
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

    [ValueConversion(typeof(double), typeof(double))]
    public class ActualWidthToWidthConverter : IValueConverter
    {
        private readonly double ScrollbarWidth = 20;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                double val = (double)value;
                val = val - ScrollbarWidth;
                return val < 0 ? 0 : val;
            }
            if (parameter is Int32)
            {
                int param = (int)parameter;
                double val = (double)value;
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

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public bool IsInvert { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return IsInvert ? value != null && !((bool)value) ? Visibility.Visible : Visibility.Collapsed :
                value != null && (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NumericToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.IsNull())
                return "";
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                              System.Globalization.CultureInfo culture)
        {
            Enum enumValue = default(Enum);
            if (parameter is Type)
            {
                enumValue = (Enum)Enum.Parse((Type)parameter, value.ToString());
            }
            return enumValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            int returnValue = 0;
            if (parameter is Type)
            {
                returnValue = (int)Enum.Parse((Type)parameter, value.ToString());
            }
            return returnValue;
        }
    }

    public class RiverStatusToColorBrushConverter : IValueConverter
    {

        private List<SolidColorBrush> _brushes;

        public RiverStatusToColorBrushConverter()
        {
            _brushes = new List<SolidColorBrush>();
            _brushes.Add(new SolidColorBrush(Colors.OrangeRed));
            _brushes.Add(new SolidColorBrush(Colors.ForestGreen));
            _brushes.Add(new SolidColorBrush(Colors.Black));
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                OFRiverStatus status = (OFRiverStatus)value;
                switch (status)
                {
                    case OFRiverStatus.Busy:
                    case OFRiverStatus.InitialIndexing:
                        return _brushes[0];
                    default:
                        return _brushes[1];
                }
            }
            catch (Exception)
            {
                return _brushes[2];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



}
