using System;

namespace WSUI.Module.Core
{
    public class KindNameIdAttribute : Attribute
    {

        public KindNameIdAttribute(string name, int id, string  icon)
        {
            Name = name;
            Id = id;
            Icon = icon;
        }

        public string Name
        {
            get; protected set;
        }

        public int Id
        {
            get; protected set;
        }

        public string Icon
        {
            get; protected set;
        }

    }
}
