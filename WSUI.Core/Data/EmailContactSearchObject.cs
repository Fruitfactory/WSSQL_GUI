using WSUI.Core.Core.Attributes;
using WSUI.Core.Enums;

namespace WSUI.Core.Data
{
    public class EmailContactSearchObject : BaseEmailSearchObject
    {
        [Field("System.Message.CcAddress", 15, false)]
        public string[] CcAddress { get; set; }

        public EmailContactSearchObject()
        {
            TypeItem = TypeSearchItem.Contact;
            Tag = "Click to email recipient";
        }

        public string EMail { get; set; }

        public override void SetValue(int index, object value)
        {
            base.SetValue(index, value);
            switch (index)
            {
                case 15:
                    CcAddress = value as string[];
                    break;
            }
        }
    }
}