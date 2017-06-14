using System;
using System.Collections.Generic;
using System.Reflection;
using OF.Core.Core.LimeLM;
using OF.Core.Enums;
using Outlook = Microsoft.Office.Interop.Outlook;
using OF.Core.Logger;
using COMException = System.Runtime.InteropServices.COMException;

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

        public static bool IsOfflineMode(this Outlook.Application app)
        {
            var ns = app.GetNamespace("MAPI");
            return ns.IsNotNull() && (ns.Offline || ns.ExchangeConnectionMode == Outlook.OlExchangeConnectionMode.olCachedDisconnected || ns.ExchangeConnectionMode == Outlook.OlExchangeConnectionMode.olCachedOffline);
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

        
        private static readonly Dictionary<int,string> cacheExEmails = new Dictionary<int, string>();

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
                        int key = $"{sender.Name}{sender.Address}".GetHashCode();
                        if (cacheExEmails.ContainsKey(key))
                        {
                            return cacheExEmails[key];
                        }
                        //Use the ExchangeUser object PrimarySMTPAddress
                        Outlook.ExchangeUser exchUser = sender.GetExchangeUser();
                        cacheExEmails[key] = exchUser != null ? exchUser.PrimarySmtpAddress : null;
                        OFLogger.Instance.LogDebug($"EX: {cacheExEmails[key]}");
                        return cacheExEmails[key];
                    }
                    catch (Exception ex)
                    {
                        OFLogger.Instance.LogError(ex.ToString());
                    }
                }
                return sender.Address; //sender.PropertyAccessor.GetProperty(PR_SMTP_ADDRESS) as string;
            }
            catch (COMException com)
            {
                if (com.ErrorCode.GetErrorCode() == 0x000006BA)
                {
                    throw;
                }
                OFLogger.Instance.LogError(com.ToString());
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
            return null;
        }
    }
}