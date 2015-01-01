using System.Windows.Input;
using WSUI.Core.Core.AdvancedSearchCriteria;

namespace WSUI.Infrastructure.AdvancedSearch
{
    public class StringAdvancedSearchCriteria : AdvancedSearchCriteria
    {
        public StringAdvancedSearchCriteria(ICommand addCommand, ICommand removeCommand) : base(addCommand, removeCommand)
        {
        }
    }
}