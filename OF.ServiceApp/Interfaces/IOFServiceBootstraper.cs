using System;

namespace OF.ServiceApp.Interfaces
{
    public interface IOFServiceBootstraper
    {

        bool IsApplicationAlreadyWorking();

        void Initialize();

        void Run();

        void Exit();
    }
}