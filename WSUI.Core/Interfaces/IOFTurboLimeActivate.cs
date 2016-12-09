using System;
using OF.Core.Enums;

namespace OF.Core.Interfaces
{
    public interface IOFTurboLimeActivate
    {
        int DaysRemain { get; }
        OFActivationState State { get; }
        void TryCheckAgain();
        void Activate(Action callback);
        bool Deactivate(bool deleteKey = false);
        void IncreaseTimeUsedFlag();
        void Init();
    }
}