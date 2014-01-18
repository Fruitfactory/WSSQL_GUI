using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace WSUI.Module.Interface
{
    public interface ILazyKind
    {
        IKindItem Kind { get; }
        string UIName { get; set; }
        string Icon{get;}
        int ID { get; }
        bool Toggle { get; set; }
        void Initialize();
        ICommand ChooseCommand { get; }

    }
}
