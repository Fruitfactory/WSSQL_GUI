using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OF.Module.Service.Dialogs.Message
{
    [ValueConversion(typeof(MessageBoxImage),typeof(string))]
    public class OFMessageImageToPictureConvert : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var messageImage = (MessageBoxImage) value;
            switch (messageImage)
            {
                case MessageBoxImage.Question:
                    return @"pack://application:,,,/OF.Module;component/Images/question.png";
                case MessageBoxImage.Warning:
                    return @"pack://application:,,,/OF.Module;component/Images/warning.png";
                case MessageBoxImage.Information:
                    return @"pack://application:,,,/OF.Module;component/Images/information.png";
                case MessageBoxImage.Error:
                    return @"pack://application:,,,/OF.Module;component/Images/error.png";
                default:
                    return @"pack://application:,,,/OF.Module;component/Images/banned.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}