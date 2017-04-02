namespace OF.Core.Interfaces
{
    public interface IOFNamedPipeObserver<in T>
    {
        void Update(T message);
    }
}