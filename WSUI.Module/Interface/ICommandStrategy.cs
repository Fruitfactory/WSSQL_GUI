using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using WSUI.Infrastructure.Service.Enums;

namespace WSUI.Module.Interface
{
    public interface ICommandStrategy
    {
        void Init();

        ObservableCollection<IWSCommand> Commands
        {
            get;
        }

        TypeSearchItem Type
        {
            get;
        }
    }
}
