using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

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
}
