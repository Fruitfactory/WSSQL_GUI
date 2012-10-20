using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSSQLGUI.Controls.ProgressControl
{
    class ProgressOperation
    {
        public string Name { get; set; }
        public int DelayTime { get; set; }
        public Action CancelAction { get; set; } 
    }
}
