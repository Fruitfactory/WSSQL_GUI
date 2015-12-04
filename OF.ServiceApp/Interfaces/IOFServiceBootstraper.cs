using System;

namespace OF.ServiceApp.Interfaces
{
    public interface IOFServiceBootstraper
    {
        void Initialize();

        void Run();

        void Exit();
    }
}