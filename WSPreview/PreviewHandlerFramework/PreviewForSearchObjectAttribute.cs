using System;
using WSUI.Core.Enums;

namespace WSPreview.PreviewHandler.PreviewHandlerFramework
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