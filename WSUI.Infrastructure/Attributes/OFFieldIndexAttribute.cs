using System;
namespace OF.Infrastructure.Attributes
{
    public class OFFieldIndexAttribute : Attribute
    {
        public OFFieldIndexAttribute(uint index)
        {
            Index = index;
        }
        public uint Index { get; private set; }
    }
}