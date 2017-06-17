using System.Net.Mail;
using OF.Core.Data.NamedPipeMessages.Response;

namespace OF.Core.Interfaces
{
    public interface IOFNamedPipeClient<in T>
    {
        OFNamedServerResponse Send(T message);
    }
}