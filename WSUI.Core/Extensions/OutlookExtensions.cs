using System;
using System.Reflection;
using OF.Core.Enums;
using Outlook = Microsoft.Office.Interop.Outlook;
using OF.Core.Logger;

namespace OF.Core.Extensions
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
                var email = string.Empty;
                switch (GlobalConst.CurrentOutlookVersion)
                {
                    case OutlookVersions.Outlook2007:
                        email = mail.SenderEmailAddress;
                        break;
                    case OutlookVersions.Outlook2010:
                    case OutlookVersions.Otlook2013:
                        Outlook.AddressEntry sender = mail.GetType().InvokeMember("Sender", BindingFlags.GetProperty, null, mail, null) as Outlook.AddressEntry;
                        email = GetEmailAddress(sender);
                        break;
                }
                return email;
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

            return addressEntry.GetEmailAddress();
        }

        public static string GetEmailAddress(this Outlook.AddressEntry sender)
        {
            if (sender == null)
            {
                return null;
            }

            try
            {
                //Now we have an AddressEntry representing the Sender
                if (sender.AddressEntryUserType == Outlook.OlAddressEntryUserType.olExchangeUserAddressEntry
                    || sender.AddressEntryUserType == Outlook.OlAddressEntryUserType.olExchangeRemoteUserAddressEntry)
                {
                    try
                    {
                        //Use the ExchangeUser object PrimarySMTPAddress
                        Outlook.ExchangeUser exchUser = sender.GetExchangeUser();
                        return exchUser != null ? exchUser.PrimarySmtpAddress : null;
                    }
                    catch (Exception ex)
                    {
                        OFLogger.Instance.LogInfo(ex.Message);
                    }
                }
                return sender.Address; //sender.PropertyAccessor.GetProperty(PR_SMTP_ADDRESS) as string;
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
            return null;
        }
    }
}