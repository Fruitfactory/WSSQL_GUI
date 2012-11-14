using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace WSUI.Module.Interface
{
    public interface IWSCommand : ICommand
    {
        Image Icon
        {
            get;
        }

        string Caption
        {
            get;
        }
    }
}
