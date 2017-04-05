namespace OF.Core.Interfaces
{
    public interface IOFNamedPipeObserver<in T>
    {
        object Update(T message);
    }
}