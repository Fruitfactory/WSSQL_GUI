using System;
namespace OF.Infrastructure.Attributes
{
    public class FieldIndexAttribute : Attribute
    {
        public FieldIndexAttribute(uint index)
        {
            Index = index;
        }
        public uint Index { get; private set; }
    }
}