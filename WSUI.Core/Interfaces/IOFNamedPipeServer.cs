namespace OF.Core.Interfaces
{
    public interface IOFNamedPipeServer<out T>
    {
        void Attach(IOFNamedPipeObserver<T> observer);
        void Deattach(IOFNamedPipeObserver<T> observer);
        void Start();
        void Stop();
    }
}