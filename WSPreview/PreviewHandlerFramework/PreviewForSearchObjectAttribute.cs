using System;
using OF.Core.Enums;

namespace OFPreview.PreviewHandler.PreviewHandlerFramework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PreviewForSearchObjectAttribute : Attribute
    {
        private OFTypeSearchItem _typeItem;

        public PreviewForSearchObjectAttribute(OFTypeSearchItem typeSearchItem)
        {
            _typeItem = typeSearchItem;
        }

        public OFTypeSearchItem Type { get {return _typeItem;} }

    }
}