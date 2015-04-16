using System;

namespace OFPreview.PreviewHandler.PreviewHandlerFramework
{
    public class KeyControlAttribute : Attribute
    {
        private ControlsKey _keyControl;

        public KeyControlAttribute(ControlsKey key)
        {
            _keyControl = key;
        }

        public ControlsKey Key
        {
            get { return _keyControl; }
        }
    }
}