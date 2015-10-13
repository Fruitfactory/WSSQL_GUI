using System.Windows.Input;
using OF.Core.Core.AdvancedSearchCriteria;
using OF.Core.Enums;

namespace OF.Infrastructure.MVVM.AdvancedSearch
{
    public class OFSortByAdvancedSearchCriteria : VariantAdvancedSearchCriteria<AdvancedSearchSortByType>
    {
        public OFSortByAdvancedSearchCriteria(ICommand addCommand, ICommand removeCommand) : base(addCommand, removeCommand)
        {
            CriteriaType = AdvancedSearchCriteriaType.SortBy;
        }
    }
}