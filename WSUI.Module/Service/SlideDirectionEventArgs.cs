using System;
using System.Windows.Media.Media3D;
using WSUI.Module.Enums;

namespace WSUI.Module.Service
{
    public class SlideDirectionEventArgs : EventArgs
    {
        public SlideDirectionEventArgs(UiSlideDirection direction)
        {
            Direction = direction;
        }

        public UiSlideDirection Direction { get; private set; }
    }
}