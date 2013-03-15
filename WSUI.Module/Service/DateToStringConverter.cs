using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WSUI.Infrastructure.Core;
using WSUI.Infrastructure.Models;

namespace WSUI.Module.Service
{
    [ValueConversion(typeof(DateTime),typeof(string))]
    public class DateToStringConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = ((DateTime) value).ToLocalTime().ToLongDateString();
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
            int count = (int) value;
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
            return count <= 1 ? string.Empty : string.Format("[{0}]", count);
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
            int size = (int) value;
            string result = string.Format("Size: {0} bytes", size);
            return result;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }

    [ValueConversion(typeof(BaseSearchData),typeof(string))]
    public class ObjectToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = string.Empty;
            var contact = value as ContactSearchData;
            if (contact != null)
            {
                
                result = string.Format("{0} {1} ({2})", contact.FirstName, contact.LastName,
                                       contact.EmailList != null && contact.EmailList.Count > 0
                                           ? contact.EmailList[0]
                                           : string.Empty);

                return result;
            }
            var email = value as EmailSearchData;
            if (email != null)
            {
                result = string.Format("{0} ({1})", email.From, email.From);
                return result;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}
