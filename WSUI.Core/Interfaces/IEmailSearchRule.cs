using System.Collections.Generic;

namespace OF.Core.Interfaces
{
    public interface IEmailSearchRule
    {
        IEnumerable<string> GetConversationId();

        void ApplyFilter(IEnumerable<string> conversationIds);
    }
}