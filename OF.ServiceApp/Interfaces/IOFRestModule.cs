namespace OF.ServiceApp.Interfaces
{
    public interface IOFRestModule
    {

        object Status(object arg);

        object Stop(object arg);

        object StartRead(object arg);

        object SuspendRead(object arg);

        object ResumeRead(object arg);

        object StopRead(object arg);
    }
}