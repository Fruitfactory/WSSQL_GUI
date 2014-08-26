namespace WSUI.Core.Core.Payload
{
    public abstract class BasePayload<T>
    {
        protected BasePayload(T arg)
        {
            Data = arg;
        }

        public T Data { get; private set; }
    }
}