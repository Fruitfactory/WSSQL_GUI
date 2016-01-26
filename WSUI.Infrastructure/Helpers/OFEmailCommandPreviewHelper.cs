using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text.RegularExpressions;
using Microsoft.Office.Interop.Outlook;
using OF.Core.Data;
using OF.Core.Data.ElasticSearch;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Infrastructure.Properties;

namespace OF.Infrastructure.Helpers
{
    public class OFEmailCommandPreviewHelper
    {

        private OFEmailCommandPreviewHelper()
        {

        }

        #region [static]

        private static Lazy<OFEmailCommandPreviewHelper> _instance = new Lazy<OFEmailCommandPreviewHelper>(() => new OFEmailCommandPreviewHelper());

        public static OFEmailCommandPreviewHelper Instance
        {
            get { return _instance.Value; }
        }

        #endregion

        #region [public]

        public MailItem CreateReplyEmail(OFEmailSearchObject emailSearchObject)
        {
            MailItem reply = OFOutlookHelper.Instance.PrepareReplyEmailItem(emailSearchObject);
            if (reply.IsNull())
            {
                throw new NullReferenceException("'Reply' email wasn't created.");
            }
            var temp = PrepareEmail(emailSearchObject);
            reply.HTMLBody = temp;
            return reply;
        }

       

        public MailItem CreateReplyAllEmail(OFEmailSearchObject emailSearchObject)
        {
            MailItem reply = OFOutlookHelper.Instance.PrepareReplyAllEmailItem(emailSearchObject);
            if (reply.IsNull())
            {
                throw new NullReferenceException("'ReplyAll' email wasn't created.");
            }
            var temp = PrepareEmail(emailSearchObject);
            reply.HTMLBody = temp;
            return reply;
        }

        public MailItem CreateForwardEmail(OFEmailSearchObject emailSearchObject)
        {
            MailItem reply = OFOutlookHelper.Instance.PrepareForwardEmailItem(emailSearchObject);
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

        private string PrepareEmail(OFEmailSearchObject emailSearchObject)
        {
            var temp = string.Empty;
            if (!string.IsNullOrEmpty(emailSearchObject.FromName) &&
                !string.IsNullOrEmpty(emailSearchObject.FromAddress))
            {
                temp = Regex.Replace(Resources.template, "{From}",
                    string.Format("{0}: {1}", emailSearchObject.FromName, emailSearchObject.FromAddress));    
            }
            else if (!string.IsNullOrEmpty(emailSearchObject.FromAddress))
            {
                temp = Regex.Replace(Resources.template, "{From}",
                    emailSearchObject.FromAddress);    
            }
            else if (string.IsNullOrEmpty(emailSearchObject.FromName))
            {
                temp = Regex.Replace(Resources.template, "{From}",
                    emailSearchObject.FromName);    
            }
            
            temp = Regex.Replace(temp, "{Sent}", emailSearchObject.DateReceived.ToLongDateString());
            temp = Regex.Replace(temp, "{To}", string.Join(";", GetRecepientString(emailSearchObject.To)));
            temp = Regex.Replace(temp, "{Subject}", emailSearchObject.Subject);
            temp = Regex.Replace(temp, "{Content}", emailSearchObject.HtmlContent);
            return temp;
        }

        private IEnumerable<string> GetRecepientString(OFRecipient[] arr)
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