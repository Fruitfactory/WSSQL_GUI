using System;

namespace OF.Core.Interfaces
{
    public interface IOFSearchThreadPool
    {
        void AddAction(Action action);
    }
}