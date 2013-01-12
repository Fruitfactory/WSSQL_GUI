using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace WSUI.Module.Interface
{
    public interface IScrollableView
    {
        ICommand ScrollChangeCommand { get; }
    }
}
