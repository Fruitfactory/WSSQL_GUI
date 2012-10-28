using System;
using System.Collections.Generic;
using System.Text;

namespace WSUI.Module.Interface
{
    public interface IDataView<T>
    {
        T Model
        {
            get;
            set;
        }
    }
}
