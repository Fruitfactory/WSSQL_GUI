using OF.Core.Enums;

namespace OF.Core.Core.AdvancedSearchCriteria
{
    public interface IAdvancedSearchCriteria
    {
        AdvancedSearchCriteriaType CriteriaType { get; }

        object Value { get; set; }

        bool RemoveButtonVisibility { get; set; }

    }
}