using System;

namespace OF.Core.Core.Attributes
{
    [System.AttributeUsage(AttributeTargets.Field)]
    public class OFEnumPrefixAttribute : BaseOFAttribute
    {
        public OFEnumPrefixAttribute(string prefix)
        {
            Prefix = prefix;
        }

        public string Prefix { get; private set; }

    }
}