using System.Windows;
using System.Windows.Controls;
using OF.Infrastructure.MVVM.AdvancedSearch;

namespace OF.Module.Service.AdvancedSearch
{
    public class AdvancedSearchTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StringTemplate { get; set; }

        public DataTemplate SortByTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is StringAdvancedSearchCriteria)
            {
                return StringTemplate;
            }
            if (item is SortByAdvancedSearchCriteria)
            {
                return SortByTemplate;
            }
            return null;
        }
    }
}