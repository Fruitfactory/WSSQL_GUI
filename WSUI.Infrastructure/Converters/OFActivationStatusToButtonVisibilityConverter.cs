using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using OF.Core.Enums;

namespace OF.Infrastructure.Converters
{
    [ValueConversion(typeof(OFActivationState), typeof(Visibility))]
    public class OFActivationStatusToButtonVisibilityConverter  : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            OFActivationState state = (OFActivationState) value;
            OFActivationButtons btn = (OFActivationButtons) parameter;
            if ((state == OFActivationState.NonActivated || state == OFActivationState.Trial || state == OFActivationState.TrialEnded) && btn == OFActivationButtons.Activate)
                return Visibility.Visible;
            if (state == OFActivationState.Error && btn == OFActivationButtons.TryAgain)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}