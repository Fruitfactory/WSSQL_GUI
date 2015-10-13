using System;
using System.Windows.Media.Media3D;
using OF.Module.Enums;

namespace OF.Module.Service
{
    public class OFSlideDirectionEventArgs : EventArgs
    {
        public OFSlideDirectionEventArgs(UiSlideDirection direction)
        {
            Direction = direction;
        }

        public UiSlideDirection Direction { get; private set; }
    }
}