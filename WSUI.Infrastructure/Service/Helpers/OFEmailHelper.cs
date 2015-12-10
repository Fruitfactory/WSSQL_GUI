using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using OF.Core.Data;
using OF.Core.Data.ElasticSearch;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Logger;
using OF.Infrastructure.Implements.ElasticSearch.Clients;

namespace OF.Infrastructure.Service.Helpers
{
    public class OFEmailHelper
    {
        public const string MAIL_EXT = "eml";
        public const string MAIL_FILTER = "*." + MAIL_EXT;


        #region [static]

        private static Lazy<OFEmailHelper> _instance = new Lazy<OFEmailHelper>(() => new OFEmailHelper());

        public static OFEmailHelper Instance
        {
            get { return _instance.Value; }
        }

        #endregion


        public string GetEmailEmlFilename(OFEmailSearchObject emailObject)
        {
            if (emailObject.IsNull())
            {
                return String.Empty;
            }

            if (OFTempFileManager.Instance.IsEmlFileExistForEmailObject(emailObject))
            {
                return OFTempFileManager.Instance.GetExistEmlFileForEmailObject(emailObject);
            }

            string result = String.Empty;
            string tempFolder = OFTempFileManager.Instance.GenerateTempFolderForObject(emailObject);
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(emailObject.FromAddress, emailObject.FromName); //emailObject.FromAddress
            if (emailObject.To != null)
            {
                emailObject.To.ForEach(r =>
                {
                    if (r.Address.IsEmail())
                        mail.To.Add(new MailAddress(r.Address, r.Name));
                });
            }
            if (emailObject.Cc != null)
            {
                emailObject.Cc.ForEach(r =>
                {
                    if (r.Address.IsEmail())
                        mail.CC.Add(new MailAddress(r.Address, r.Name));
                });
            }
            if (emailObject.Bcc != null)
            {
                emailObject.Bcc.ForEach(r =>
                {
                    if (r.Address.IsEmail())
                        mail.Bcc.Add(new MailAddress(r.Address, r.Name));
                });
            }

            if (!String.IsNullOrEmpty(emailObject.Content))
            {
                mail.Body = emailObject.Content;
            }
            else
            {
                mail.Body = emailObject.HtmlContent;
                mail.IsBodyHtml = true;
            }
            mail.Subject = emailObject.Subject;
            if (emailObject.Attachments != null)
            {
                var fileList = GetAttachments(emailObject, tempFolder);
                foreach (var filename in fileList)
                {
                    mail.Attachments.Add(new Attachment(filename));
                }
            }
            try
            {
                result = SaveToEML(mail, tempFolder);
                OFTempFileManager.Instance.SetEmlFileForEmailObject(emailObject, result);
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
            return result;
        }



        private IEnumerable<string> GetAttachments(OFEmailSearchObject searchObj, string tempFolder)
        {
            var esClient = new OFElasticSearchClient();
            var result = esClient.Search<OFAttachmentContent>(s => s.Query(d => d.QueryString(qq => qq.Query(searchObj.EntryID))));
            var fileList = new List<string>();
            if (result.Documents.Any())
            {
                foreach (var attachment in result.Documents)
                {
                    try
                    {
                        byte[] content = Convert.FromBase64String(attachment.Content);
                        var filename = String.Format("{0}/{1}", tempFolder, attachment.Filename);
                        File.WriteAllBytes(filename, content);
                        fileList.Add(filename);

                    }
                    catch (Exception exception)
                    {
                        OFLogger.Instance.LogError(exception.Message);
                    }
                }
            }
            return fileList;
        }

        private string SaveToEML(MailMessage msg, string tempFolder)
        {
            string result = String.Empty;
            using (var client = new SmtpClient())
            {
                client.UseDefaultCredentials = true;
                client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                client.PickupDirectoryLocation = tempFolder;
                client.Send(msg);
            }
            try
            {
                result = Directory.GetFiles(tempFolder, MAIL_FILTER).Single();
                string filename = Path.Combine(tempFolder, Path.GetFileNameWithoutExtension(Path.GetTempFileName()) + "." + MAIL_EXT);
                File.Copy(result, filename);
                File.Delete(result);
                result = filename;
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(e.Message);
            }
            return result;
        }
    }
}