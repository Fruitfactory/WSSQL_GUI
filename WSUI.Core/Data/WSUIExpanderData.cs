using System.Windows;
using System.Windows.Controls;

namespace OF.Core.Data
{
    public class OFExpanderData
    {
        public ExpandDirection ExpandDirection { get; set; }

        public Size NewSize { get; set; }

        public Size OldSize { get; set; }

        public Point Location { get; set; }

        public bool IsScrollBarVisible { get; set; }

        public bool IsVisibleOne { get; set; }

    }
}