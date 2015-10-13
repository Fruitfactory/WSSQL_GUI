using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using OF.Core.Enums;

namespace OF.Infrastructure.Converters
{
    [ValueConversion(typeof(OFActivationState),typeof(Visibility))]
    public class OFActivationStatusToGridVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            OFActivationState state = (OFActivationState) value;
            switch (state)
            {
                case OFActivationState.Activated:
                    return Visibility.Collapsed;
                case OFActivationState.Error:
                case OFActivationState.NonActivated:
                case OFActivationState.Trial:
                case OFActivationState.TrialEnded:
                    return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}