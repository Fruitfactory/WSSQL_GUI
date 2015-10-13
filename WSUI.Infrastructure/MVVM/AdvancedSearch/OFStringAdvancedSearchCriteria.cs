using System.Windows.Input;
using OF.Core.Core.AdvancedSearchCriteria;

namespace OF.Infrastructure.MVVM.AdvancedSearch
{
    public class OFStringAdvancedSearchCriteria : OFAdvancedSearchCriteria
    {
        public OFStringAdvancedSearchCriteria(ICommand addCommand, ICommand removeCommand) : base(addCommand, removeCommand)
        {
        }
    }
}