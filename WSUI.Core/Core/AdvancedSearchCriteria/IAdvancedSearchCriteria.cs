using WSUI.Core.Enums;

namespace WSUI.Core.Core.AdvancedSearchCriteria
{
    public interface IAdvancedSearchCriteria
    {
        AdvancedSearchCriteriaType CriteriaType { get; }

        object Value { get; set; }

        bool RemoveButtonVisibility { get; set; }

    }
}