using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WSUI.Core.Enums;

namespace WSUI.Infrastructure.Converters
{
    [ValueConversion(typeof(ActivationState), typeof(Visibility))]
    public class ActivationStatusToButtonVisibilityConverter  : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ActivationState state = (ActivationState) value;
            ActivationButtons btn = (ActivationButtons) parameter;
            if ((state == ActivationState.NonActivated || state == ActivationState.Trial || state == ActivationState.TrialEnded) && btn == ActivationButtons.Activate)
                return Visibility.Visible;
            if (state == ActivationState.Error && btn == ActivationButtons.TryAgain)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}