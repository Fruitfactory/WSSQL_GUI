using OF.Core.Core.Attributes;
using OF.Core.Enums;

namespace OF.Core.Data
{
    public class OFEmailContactSearchObject : OFBaseEmailSearchObject
    {

        public OFEmailContactSearchObject()
        {
            TypeItem = OFTypeSearchItem.Contact;
            Tag = "Click for details";
        }

        public string EMail { get; set; }

        public string ContactName { get; set; }

        public string AddressType { get; set; }

    }
}