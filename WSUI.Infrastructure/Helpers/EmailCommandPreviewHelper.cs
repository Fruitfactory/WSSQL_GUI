using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Office.Interop.Outlook;
using WSUI.Core.Data;
using WSUI.Core.Data.ElasticSearch;
using WSUI.Core.Extensions;
using WSUI.Core.Helpers;
using WSUI.Infrastructure.Properties;

namespace WSUI.Infrastructure.Helpers
{
    public class EmailCommandPreviewHelper
    {

        private EmailCommandPreviewHelper()
        {

        }

        #region [static]

        private static Lazy<EmailCommandPreviewHelper> _instance = new Lazy<EmailCommandPreviewHelper>(() => new EmailCommandPreviewHelper());

        public static EmailCommandPreviewHelper Instance
        {
            get { return _instance.Value; }
        }

        #endregion

        #region [public]

        public MailItem CreateReplyEmail(EmailSearchObject emailSearchObject)
        {
            MailItem reply = OutlookHelper.Instance.PrepareReplyEmailItem(emailSearchObject);
            if (reply.IsNull())
            {
                throw new NullReferenceException("'Reply' email wasn't created.");
            }
            var temp = PrepareEmail(emailSearchObject);
            reply.HTMLBody = temp;
            return reply;
        }

       

        public MailItem CreateReplyAllEmail(EmailSearchObject emailSearchObject)
        {
            MailItem reply = OutlookHelper.Instance.PrepareReplyAllEmailItem(emailSearchObject);
            if (reply.IsNull())
            {
                throw new NullReferenceException("'ReplyAll' email wasn't created.");
            }
            var temp = PrepareEmail(emailSearchObject);
            reply.HTMLBody = temp;
            return reply;
        }

        public MailItem CreateForwardEmail(EmailSearchObject emailSearchObject)
        {
            MailItem reply = OutlookHelper.Instance.PrepareForwardEmailItem(emailSearchObject);
            if (reply.IsNull())
            {
                throw new NullReferenceException("'Forward' email wasn't created.");
            }
            var temp = PrepareEmail(emailSearchObject);
            reply.HTMLBody = temp;
            return reply;
        }

        #endregion

        #region [private]

        private string PrepareEmail(EmailSearchObject emailSearchObject)
        {
            string temp = Regex.Replace(Resources.template, "{From}",
                string.Format("{0}: {1}", emailSearchObject.FromName, emailSearchObject.FromAddress));
            temp = Regex.Replace(temp, "{Sent}", emailSearchObject.DateReceived.ToLongDateString());
            temp = Regex.Replace(temp, "{To}", string.Join(";", GetRecepientString(emailSearchObject.To)));
            temp = Regex.Replace(temp, "{Subject}", emailSearchObject.Subject);
            temp = Regex.Replace(temp, "{Content}", emailSearchObject.HtmlContent);
            return temp;
        }

        private IEnumerable<string> GetRecepientString(WSUIRecipient[] arr)
        {
            if (arr.IsNull())
            {
                return default(IEnumerable<string>);
            }
            return arr.Select(wsuiRecipient => string.Format("{0} ({1})", wsuiRecipient.Name, wsuiRecipient.Address)).ToList();
        }


        #endregion

    }
}