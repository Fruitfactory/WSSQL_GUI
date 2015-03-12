using WSUI.Core.Core.Attributes;
using WSUI.Core.Enums;

namespace WSUI.Core.Data
{
    public class EmailContactSearchObject : BaseEmailSearchObject
    {
        
        public string[] CcAddress { get; set; }

        public string[] ToName { get; set; }

        public string[] CcName { get; set; }

        public EmailContactSearchObject()
        {
            TypeItem = TypeSearchItem.Contact;
            Tag = "Click to email recipient";
        }

        public string EMail { get; set; }

        public string ContactName { get; set; }

    }
}