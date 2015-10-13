using System;
using System.Globalization;
using System.Windows.Data;
using OF.Core.Enums;
using OF.Core.Logger;

namespace OF.Infrastructure.Converters
{
    public class OFAdvancedSearchTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var advancedSearchType = (AdvancedSearchCriteriaType)value;
                return advancedSearchType.ToString();
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
            return AdvancedSearchCriteriaType.None.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}