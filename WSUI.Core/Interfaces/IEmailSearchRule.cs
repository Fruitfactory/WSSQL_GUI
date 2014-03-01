using System.Collections;
using System.Collections.Generic;

namespace WSUI.Core.Interfaces
{
    public interface IEmailSearchRule
    {
        IEnumerable<string> GetConversationId();
        void ApplyFilter(IEnumerable<string> conversationIds);
    }
}