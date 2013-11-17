using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WSUI.Core.Enums;

namespace WSUI.Infrastructure.Converters
{
    [ValueConversion(typeof(ActivationState),typeof(Visibility))]
    public class ActivationStatusToGridVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ActivationState state = (ActivationState) value;
            switch (state)
            {
                case ActivationState.Activated:
                    return Visibility.Collapsed;
                case ActivationState.Error:
                case ActivationState.NonActivated:
                case ActivationState.Trial:
                case ActivationState.TrialEnded:
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