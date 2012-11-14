using System;
using System.Collections.Generic;
using System.Text;
using WSUI.Infrastructure.Service.Enums;

namespace WSUI.Module.Interface
{
    public interface ICommandStrategy
    {
        void Init();

        List<IWSCommand> Commands
        {
            get;
        }

        TypeSearchItem Type
        {
            get;
        }
    }
}
