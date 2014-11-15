using System.Windows;
using System.Windows.Controls;

namespace WSUI.Core.Data
{
    public class WSUIExpanderData
    {
        public ExpandDirection ExpandDirection { get; set; }

        public Size NewSize { get; set; }

        public Size OldSize { get; set; }

        public Point Location { get; set; }

        public bool IsScrollBarVisible { get; set; }

    }
}