using System;
using OF.Core.Enums;

namespace OFPreview.PreviewHandler.PreviewHandlerFramework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PreviewForSearchObjectAttribute : Attribute
    {
        private TypeSearchItem _typeItem;

        public PreviewForSearchObjectAttribute(TypeSearchItem typeSearchItem)
        {
            _typeItem = typeSearchItem;
        }

        public TypeSearchItem Type { get {return _typeItem;} }

    }
}