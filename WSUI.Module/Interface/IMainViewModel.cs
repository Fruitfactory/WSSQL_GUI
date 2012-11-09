using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSUI.Module.Interface
{
    public interface IMainViewModel
    {
        event EventHandler Start;
        event EventHandler Complete;
    }
}
