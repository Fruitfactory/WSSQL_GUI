using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSUI.Module.Core
{
    public class KindNameIdAttribute : Attribute
    {

        public KindNameIdAttribute(string name, int id)
        {
            Name = name;
            Id = id;

        }

        public string Name
        {
            get; protected set;
        }

        public int Id
        {
            get; protected set;
        }
    }
}
