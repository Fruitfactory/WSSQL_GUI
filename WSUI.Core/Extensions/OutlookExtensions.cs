using System;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace WSUI.Core.Extensions
{
    public static class OutlookExtensions
    {
        private const string PR_SMTP_ADDRESS = @"http://schemas.microsoft.com/mapi/proptag/0x39FE001E";

        public static string GetSenderSMTPAddress(this Outlook.MailItem mail)
        {
            if (mail == null)
            {
                throw new ArgumentNullException();
            }
            if (mail.SenderEmailType == "EX")
            {
                Outlook.AddressEntry sender = mail.Sender;
                return GetEmailAddress(sender);
            }
            else
            {
                return mail.SenderEmailAddress;
            }
        }

        public static string GetSMTPAddress(this Outlook.Recipient recipient)
        {
            if (recipient == null)
            {
                throw new ArgumentException("recipient");
            }
            Outlook.AddressEntry addressEntry = recipient.AddressEntry;

            return GetEmailAddress(addressEntry);
        }

        private static string GetEmailAddress(Outlook.AddressEntry sender)
        {
            if (sender != null)
            {
                //Now we have an AddressEntry representing the Sender
                if (sender.AddressEntryUserType == Outlook.OlAddressEntryUserType.olExchangeUserAddressEntry
                    || sender.AddressEntryUserType == Outlook.OlAddressEntryUserType.olExchangeRemoteUserAddressEntry)
                {
                    //Use the ExchangeUser object PrimarySMTPAddress
                    Outlook.ExchangeUser exchUser = sender.GetExchangeUser();
                    if (exchUser != null)
                    {
                        return exchUser.PrimarySmtpAddress;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return sender.Address; //sender.PropertyAccessor.GetProperty(PR_SMTP_ADDRESS) as string;
                }
            }
            else
            {
                return null;
            }
        }
    }
}