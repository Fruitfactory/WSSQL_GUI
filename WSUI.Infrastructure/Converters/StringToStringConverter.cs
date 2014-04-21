using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using WSUI.Core.Extensions;

namespace WSUI.Infrastructure.Converters
{
    [ValueConversion(typeof(string),typeof(string))]
    public class StringToStringConverter : IValueConverter
    {
        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (string.IsNullOrEmpty((string)value))
                return string.Empty;
            var result = ((string)value).ConvertToMostEfficientEncoding();
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Empty;
        }
    }
}
