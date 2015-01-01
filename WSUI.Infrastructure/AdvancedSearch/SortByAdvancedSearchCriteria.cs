using System.Windows.Input;
using WSUI.Core.Core.AdvancedSearchCriteria;
using WSUI.Core.Enums;

namespace WSUI.Infrastructure.AdvancedSearch
{
    public class SortByAdvancedSearchCriteria : VariantAdvancedSearchCriteria<AdvancedSearchSortByType>
    {
        public SortByAdvancedSearchCriteria(ICommand addCommand, ICommand removeCommand) : base(addCommand, removeCommand)
        {
            CriteriaType = AdvancedSearchCriteriaType.SortBy;
        }
    }
}