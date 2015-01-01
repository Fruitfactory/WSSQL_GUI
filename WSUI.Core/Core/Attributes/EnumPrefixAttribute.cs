using System;

namespace WSUI.Core.Core.Attributes
{
    [System.AttributeUsage(AttributeTargets.Field)]
    public class EnumPrefixAttribute : BaseWSUIAttribute
    {
        public EnumPrefixAttribute(string prefix)
        {
            Prefix = prefix;
        }

        public string Prefix { get; private set; }

    }
}