using WSUI.Core.Core.Attributes;
using WSUI.Core.Enums;

namespace WSUI.Core.Data
{
    public class EmailContactSearchObject : BaseEmailSearchObject
    {
        [Field("System.Message.FromAddress",10,false)]
        public string[] FromAddress { get; set; }

        [Field("System.Message.CcAddress",11,false)]
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
                case 10:
                    FromAddress = value as string[];
                    break;
                case 11:
                    CcAddress = value as string[];
                    break;
            }
        }
    }
}