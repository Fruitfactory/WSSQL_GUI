///////////////////////////////////////////////////////////
//  ContactSearchObject.cs
//  Implementation of the Class ContactSearchObject
//  Generated by Enterprise Architect
//  Created on:      27-Sep-2013 1:33:34 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////

using WSUI.Core.Core.Attributes;
using WSUI.Core.Enums;

namespace WSUI.Core.Data
{
    public class ContactSearchObject : BaseSearchObject
    {
        [Field("System.Contact.FirstName", 7, false)]
        public string FirstName { get; set; }

        [Field("System.Contact.LastName", 8, false)]
        public string LastName { get; set; }

        [Field("System.Contact.EmailAddress", 9, false)]
        public string EmailAddress { get; set; }

        [Field("System.Contact.EmailAddress2", 10, false)]
        public string EmailAddress2 { get; set; }

        [Field("System.Contact.EmailAddress3", 11, false)]
        public string EmailAddress3 { get; set; }

        [Field("System.Contact.BusinessTelephone", 12, false)]
        public string BusinessTelephone { get; set; }

        [Field("System.Contact.HomeTelephone", 13, false)]
        public string HomeTelephone { get; set; }

        [Field("System.Contact.MobileTelephone", 14, false)]
        public string MobileTelephone { get; set; }

        public ContactSearchObject()
        {
            TypeItem = TypeSearchItem.Contact;
            Tag = "Click to email recipient";
        }

        public override void SetValue(int index, object value)
        {
            base.SetValue(index, value);
            switch (index)
            {
                case 7:
                    FirstName = value as string;
                    break;
                case 8:
                    LastName = value as string;
                    break;
                case 9:
                    EmailAddress = value as string;
                    break;
                case 10:
                    EmailAddress2 = value as string;
                    break;
                case 11:
                    EmailAddress3 = value as string;
                    break;
                case 12:
                    BusinessTelephone = value as string;
                    break;
                case 13:
                    HomeTelephone = value as string;
                    break;
                case 14:
                    MobileTelephone = value as string;
                    break;
            }
        }

        public string GetEmail()
        {
            return !string.IsNullOrEmpty(EmailAddress)
                ? EmailAddress
                : !string.IsNullOrEmpty(EmailAddress2)
                    ? EmailAddress2
                    : !string.IsNullOrEmpty(EmailAddress3)
                        ? EmailAddress3
                        : string.Empty;
        }
    }//end ContactSearchObject
}//end namespace Data