using System;

namespace WSUI.Module.Core
{
    public class KindNameIdAttribute : Attribute
    {

        public KindNameIdAttribute(string name, int id, string  icon,string data)
        {
            Name = name;
            Id = id;
            Icon = icon;
            Data = data;
            IsVisibleByDefault = true;
        }

        public KindNameIdAttribute(string name, int id, string icon, string data, bool isVisibleByDefault)
        {
            Name = name;
            Id = id;
            Icon = icon;
            Data = data;
            IsVisibleByDefault = isVisibleByDefault;
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

        public string Data { get; private set; }

        public bool IsVisibleByDefault { get; private set; }


    }
}
