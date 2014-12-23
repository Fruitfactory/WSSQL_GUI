using System.Windows.Input;
using WSUI.Module.Enums;

namespace WSUI.Module.Interface.Service
{
    public interface IAdvancedSearchCriteria
    {
        AdvancedSearchCriteriaType CriteriaType { get; }

        string Value { get; set; }

        bool RemoveButtonVisibility { get; set; }

    }
}