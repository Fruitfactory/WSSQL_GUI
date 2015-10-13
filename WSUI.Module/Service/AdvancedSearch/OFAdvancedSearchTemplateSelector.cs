using System.Windows;
using System.Windows.Controls;
using OF.Infrastructure.MVVM.AdvancedSearch;

namespace OF.Module.Service.AdvancedSearch
{
    public class OFAdvancedSearchTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StringTemplate { get; set; }

        public DataTemplate SortByTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is OFStringAdvancedSearchCriteria)
            {
                return StringTemplate;
            }
            if (item is OFSortByAdvancedSearchCriteria)
            {
                return SortByTemplate;
            }
            return null;
        }
    }
}