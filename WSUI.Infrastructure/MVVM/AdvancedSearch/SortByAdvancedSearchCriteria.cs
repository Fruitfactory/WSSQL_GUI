using System.Windows.Input;
using OF.Core.Core.AdvancedSearchCriteria;
using OF.Core.Enums;

namespace OF.Infrastructure.MVVM.AdvancedSearch
{
    public class SortByAdvancedSearchCriteria : VariantAdvancedSearchCriteria<AdvancedSearchSortByType>
    {
        public SortByAdvancedSearchCriteria(ICommand addCommand, ICommand removeCommand) : base(addCommand, removeCommand)
        {
            CriteriaType = AdvancedSearchCriteriaType.SortBy;
        }
    }
}