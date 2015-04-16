using System;

namespace OF.Core.Core.Attributes
{
    [System.AttributeUsage(AttributeTargets.Field)]
    public class EnumPrefixAttribute : BaseOFAttribute
    {
        public EnumPrefixAttribute(string prefix)
        {
            Prefix = prefix;
        }

        public string Prefix { get; private set; }

    }
}