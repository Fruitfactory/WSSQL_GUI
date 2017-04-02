using System.Net.Mail;

namespace OF.Core.Interfaces
{
    public interface IOFNamedPipeClient<in T>
    {
        void Send(T message);
    }
}