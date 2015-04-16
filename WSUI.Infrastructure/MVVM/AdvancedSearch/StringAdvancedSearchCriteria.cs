using System.Windows.Input;
using OF.Core.Core.AdvancedSearchCriteria;

namespace OF.Infrastructure.MVVM.AdvancedSearch
{
    public class StringAdvancedSearchCriteria : AdvancedSearchCriteria
    {
        public StringAdvancedSearchCriteria(ICommand addCommand, ICommand removeCommand) : base(addCommand, removeCommand)
        {
        }
    }
}