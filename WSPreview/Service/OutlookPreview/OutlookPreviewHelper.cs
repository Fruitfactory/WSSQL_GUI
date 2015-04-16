using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using GDIDraw.Service;
using HtmlAgilityPack;
using OFPreview.PreviewHandler.PreviewHandlerFramework;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data;
using OF.Core.Data.ElasticSearch;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Logger;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace OFPreview.PreviewHandler.Service.OutlookPreview
{
    public sealed class OutlookPreviewHelper : IOutlookPreviewHelper
    {
        #region [needs]

        private static char[] Symbol = new char[] { '@', '.', ',' };
        private const string AfterStrongTemplateBegin = "<font style='background-color: yellow'><strong>";
        private const string AfterStrongTemplateEnd = "</strong></font>";
        private const string OutlookProcessName = "OUTLOOK";
        private const string OutlookApplication = "Outlook.Application";
        private const string WordRegex = @"(?<word>[\/\$\d\w\.]+)";
        private const string ExtOfImage = "png";
        private const string BodyTag = "<body";
        private const string NAEmpty = "<n/a>";

        #region pattern for html

        private const string PageBegin = @"<!DOCTYPE html><html lang='en'><head><title></title><meta charset='utf-8' http-equiv='Content-Type' content='text/html; charset=utf-8;'><style type='text/css'>.style1{color: gray;word-wrap: break-word}.style2{width:100%;}</style></head><body style='font-family: Arial, Helvetica, sans-serif;font-size: x-small;word-wrap: break-word;;white-space: nowrap;'>";
        private const string TableBegin = @"<table style='width: 100%; table-layout: auto;'>";
        private const string SubjectRow = @"<tr><td  style='color: #0c0202; font-size: small; margin: 5px 5px 5px 5px' colspan='2'>{0}</td></tr>";
        private const string SenderRow = @"<tr><td class='style1'>From:</td><td class='style2'>{0}</td></tr>";// @"<tr><td  style='color: #0c0202; font-size: small; margin: 5px 5px 5px 5px' colspan='2'>{0}</td></tr>";
        private const string ToRow = @"<tr><td class='style1'>To:</td><td class='style2'>{0}</td></tr>";
        private const string CCRow = @"<tr><td class='style1'>CC:</td><td class='style2'>{0}</td></tr>";
        private const string InRow = @"<tr><td class='style1'>In:</td><td ><a href='uuid:{0}'>{1}</a></td></tr>";
        private const string AttachmentsRow = @"<tr><td class='style1'>Attachments:</td><td >{0}</td></tr>";
        private const string SendRow = @"<tr><td class='style1'>Sent:</td><td >{0}</td></tr>";

        private const string EmailRow = @"<tr style='margin: 25px 10px 10px 10px'><td colspan='2' ><hr /><html><body style='word-wrap: break-word;white-space: nowrap;font-size:x-small' >{0}</body></html></td></tr>";
        private const string TableEnd = @"</table>";
        private const string PageEnd = @"</body></html>";

        //appointment
        private const string StartRow = @"<tr><td class='style1'>Start:</td><td class='style2'>{0}</td></tr>";

        private const string EndRow = @"<tr><td class='style1'>End:</td><td class='style2'>{0}</td></tr>";
        private const string RequiredRow = @"<tr><td class='style1'>Required Attendees:</td><td class='style2'>{0}</td></tr>";
        private const string LocationRow = @"<tr><td class='style1'>Location: </td><td class='style2'><a href='{0}'>{0}</a></td></tr>";
        private const string MailTo = @"<a href='mailto:{0}'>{0}</a> ";
        private const string MailToWithName = @"<a href='mailto:{0}'>{1}</a> ";
        private const string ContactName = @"<a href='fax:{0}'>{0}</a> ";
        private const string ContactNameWithEmail = @"<a href='fax:{0}'>{1}</a> ";

        // meeting
        private const string TopicRow = @"<tr><td class='style1'>Topic:</td><td class='style2'>{0}</td></tr>";

        private const string LinkTemplate = @"<img src='{1}' width='16' height='16' /><a href='{0}'>{2}</a>&nbsp;&nbsp;&nbsp;";

        private const string HtmlTagOpenName = "html";
        private const string HtmlTagcloseName = "/html";

        private const string HtmlTemplate = "<html xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='rn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-microsoft-com:office:word' xmlns:m='http://schemas.microsoft.com/office/2004/12/omml' xmlns='http://www.w3.org/TR/REC-html40'><head></head><body>{0}</body></html>";

        #endregion pattern for html

        private bool _IsExistProcess = false;
        private readonly Dictionary<string, string> _dictTempFile = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _dictImage = new Dictionary<string, string>();
        private string[] _itemArray = null;
        private string _hitString = string.Empty;
        private Outlook._Application _outlook;

        #endregion [needs]

        #region [static]

        private static readonly Lazy<OutlookPreviewHelper> _Instance = new Lazy<OutlookPreviewHelper>(() =>
                                                                                                          {
                                                                                                              var helper = new OutlookPreviewHelper();
                                                                                                              return helper;
                                                                                                          });

        #endregion [static]

        #region [static istance]

        public static OutlookPreviewHelper Instance
        {
            get { return _Instance.Value; }
        }

        #endregion [static istance]

        #region [ctor]

        private OutlookPreviewHelper()
        {
            PreviewHostType = HostType.Unknown;
        }

        #endregion [ctor]

        #region [init]

        private void Init()
        {
        }

        #endregion [init]

        #region [hit string]

        public string HitString
        {
            get { return _hitString; }
            set { _hitString = value; CreateWordsList(_hitString); }
        }

        #endregion [hit string]

        #region [full folder path]

        public string FullFolderPath { get; set; }

        #endregion [full folder path]

        #region [host type]

        public HostType PreviewHostType { get; set; }

        #endregion [host type]

        #region [Outlook app]

        public Outlook._Application OutlookApp
        {
            get
            {
                if (PreviewHostType == HostType.Application && _outlook == null)
                    _outlook = GetApplication();
                return _outlook;
            }
            set
            {
                _outlook = value;
                _IsExistProcess = true;
                PreviewHostType = HostType.Plugin;
            }
        }

        #endregion [Outlook app]

        #region [get outlook app (for app host)]

        private Outlook._Application GetApplication()
        {
            Outlook._Application ret = GetFromProcess();
            if (ret == null)
            {
                ret = CreateOutlookApplication();
            }

            return ret;
        }

        private Outlook._Application GetFromProcess()
        {
            Outlook._Application ret = null;
            try
            {
                var outlook = Process.GetProcesses().Where(p => p.ProcessName.ToUpper().StartsWith(OutlookProcessName));
                if (outlook.Count() > 0)
                {
                    ret = Marshal.GetActiveObject(OutlookApplication) as Outlook._Application;
                    _IsExistProcess = true;
                }
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(ex.Message);
            }

            return ret;
        }

        private Outlook._Application CreateOutlookApplication()
        {
            Outlook._Application ret = null;
            try
            {
                ret = new Outlook.Application();
                Outlook.NameSpace ns = ret.GetNamespace("MAPI");
                ns.Logon(Type.Missing, "", Type.Missing, Type.Missing);//ret.DefaultProfileName
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(ex.Message);
            }

            return ret;
        }

        public void CloseApplication()
        {
            if (!_IsExistProcess)
            {
                _outlook.Quit();
                Marshal.ReleaseComObject(_outlook);
            }
        }

        #endregion [get outlook app (for app host)]

        #region [strign process]

        private void CreateWordsList(string inputSequence)
        {
            if (string.IsNullOrEmpty(inputSequence))
            {
                _itemArray = new string[] { inputSequence };
                return;
            }

            MatchCollection col = Regex.Matches(inputSequence, WordRegex);
            if (col.Count == 0)
            {
                _itemArray = new string[] { inputSequence };
                return;
            }
            var list = new List<string>();
            for (int i = 0; i < col.Count; i++)
            {
                var item = col[i];
                if (string.IsNullOrEmpty(item.Value))
                    continue;
                list.Add(item.Value);
            }

            _itemArray = list.ToArray();
        }

        private string HighlightSearchString(string inputString)
        {
            if (string.IsNullOrEmpty(HitString) || string.IsNullOrEmpty(inputString) || inputString.IsHtmlComment() || _itemArray == null)
                return inputString;
            string result = inputString;

            foreach (var s in _itemArray)
            {
                var temp = Regex.Escape(s.ClearString());
                Match m = s.IsAmount() ? Regex.Match(result, string.Format(@"{0}", temp), RegexOptions.IgnoreCase) :
                    Regex.Match(result, string.Format("{0}", temp), RegexOptions.IgnoreCase);//@"\b({0})\b"

                if (m.Success)
                {
                    var partBegin = result.Substring(0, m.Index);
                    var partMath = result.Substring(m.Index, m.Length);
                    var partEnd = HighlightSearchString(result.Substring(m.Index + m.Length));
                    result = partBegin + AfterStrongTemplateBegin + partMath + AfterStrongTemplateEnd + partEnd;
                }
            }
            return result;
        }

        private string GetHtmlBodyHightlight(string body)
        {
            HtmlDocument htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(body);
            HightlightAllNodes(htmlDoc.DocumentNode);
            return htmlDoc.DocumentNode.InnerHtml.IndexOf(BodyTag) > -1 ? GetContentOfTag(htmlDoc.DocumentNode) : htmlDoc.DocumentNode.InnerHtml;
        }

        private string GetContentOfTag(HtmlNode node, string tagName = "body")
        {
            if (node == null)
                return string.Empty;
            if (node.Name == tagName)
            {
                if (!node.Attributes.Any(a => a.Name.ToUpperInvariant() == "STYLE"))
                {
                    node.Attributes.Add("style", "word-break: break-all;white-space: nowrap;");
                }
                else
                {
                    node.Attributes["style"].Value = "word-break: break-all;white-space: nowrap;";
                }
                return node.InnerHtml;
            }
            string path = string.Empty;
            foreach (var child in node.ChildNodes)
            {
                path = GetContentOfTag(child);
                if (string.IsNullOrEmpty(path))
                    continue;
                break;
            }
            return path;
        }

        private void HightlightAllNodes(HtmlNode node)
        {
            if (node == null)
                return;
            if (node.ChildNodes.Count == 0 && (node as HtmlTextNode) != null)
            {
                var nodeText = node as HtmlTextNode;
                nodeText.Text = HighlightSearchString(nodeText.Text);
                return;
            }
            foreach (var childNode in node.ChildNodes)
            {
                if (childNode.Name.ToLowerInvariant() == Extensions.LinkTag)
                {
                    childNode.RemoveTargetFromTagA();
                }
                HightlightAllNodes(childNode);
            }
        }

        #endregion [strign process]

        #region [email process]

        private dynamic GetMail(string filename)
        {
            try
            {
                if (_outlook == null)
                    return null;
                dynamic mail = _outlook.CreateItemFromTemplate(filename);

                if (mail == null)
                    return null;
                return mail;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string GetAttachments(dynamic item, string filename)
        {
            if (item.Attachments.Count > 0)
            {
                var tempFolder = Path.GetDirectoryName(filename);

                var urls = string.Empty;
                foreach (Outlook.Attachment att in item.Attachments)
                {
                    // TODO add functionality, if we don't have image for samo file ext (need to show blank image)
                    var destname = GetAttachmentValue(att, tempFolder);
                    if (!string.IsNullOrEmpty(destname))
                        urls += string.Format(LinkTemplate, att.DisplayName, destname, HighlightSearchString(att.DisplayName));
                }
                return string.Format(AttachmentsRow, urls);
            }
            return null;
        }

        private string GetAttachments(EmailSearchObject searchObj)
        {
            var esClient = new OFElasticSearchClient();
            var result = esClient.Search<OFAttachmentContent>(s => s.Query(d => d.QueryString(qq => qq.Query(searchObj.EntryID))));
            if (result.Documents.Any())
            {
                var tempFolder = TempFileManager.Instance.GenerateTempFolderForObject(searchObj);
                var urls = string.Empty;
                foreach (var attachment in result.Documents)
                {
                    try
                    {
                        byte[] content = Convert.FromBase64String(attachment.Content);
                        var filename = string.Format("{0}/{1}", tempFolder, attachment.Filename);
                        File.WriteAllBytes(filename, content);
                        if (!string.IsNullOrEmpty(filename))
                            urls += string.Format(LinkTemplate, attachment.Filename, filename, HighlightSearchString(attachment.Filename));

                    }
                    catch (Exception exception)
                    {
                        WSSqlLogger.Instance.LogError(exception.Message);
                    }
                }
                return string.Format(AttachmentsRow, urls);
            }
            return string.Empty;
        }



        public string GetPathForEmail(Uri url, string filename)
        {
            if (_outlook == null || string.IsNullOrEmpty(filename))
                return string.Empty;

            Outlook._Application app = _outlook;
            if (app == null)
                return string.Empty;
            dynamic mail = GetMail(filename);
            if (mail == null)
                return string.Empty;

            if (mail.Attachments.Count == 0)
                return string.Empty;
            string currentname = url.LocalPath;

            string path = string.Empty;
            foreach (Outlook.Attachment att in mail.Attachments)
            {
                if (att.DisplayName.Contains(currentname.Trim()))
                {
                    path = GetTempFileName(att.FileName);
                    att.SaveAsFile(path);
                    break;
                }
            }
            Marshal.ReleaseComObject(mail);
            return File.Exists(path) ? path : string.Empty;
        }

        #endregion [email process]

        #region [service helpers]

        private string GetTempFileName(string filename)
        {
            if (_dictTempFile.ContainsKey(filename))
                return _dictTempFile[filename];
            string path = string.Format("{0}{1}", Path.GetTempPath(), filename);
            _dictTempFile.Add(filename, path);
            return path;
        }

        public void DeleteAllTempFile()
        {
            if (_dictTempFile.Count == 0)
                return;

            foreach (var pair in _dictTempFile)
            {
                if (File.Exists(pair.Value))
                    File.Delete(pair.Value);
            }
            _dictTempFile.Clear();
        }

        #endregion [service helpers]

        #region [generate privew]

        private string GetBeginingOfPreview(dynamic item)
        {
            string page = PageBegin + TableBegin;
            string subject = string.Empty;
            if (item is Outlook.MailItem)
            {
                subject = ((string)(item as Outlook.MailItem).Subject);
            }
            else
            {
                subject = ((string)item.Subject);
            }
            page += string.Format(SubjectRow, HighlightSearchString(subject.DecodeString()));
            return page;
        }

        public string GetPreviewForEmail(Outlook.MailItem mail, string filename)
        {
            string page = GetBeginingOfPreview(mail);
            var senderEmailAddress = mail.GetSenderSMTPAddress();
            var clearSenderName = mail.SenderName.ClearString();
            var clearsenderEmail = senderEmailAddress.ClearString();
            page += string.Format(SenderRow, IsEmail(senderEmailAddress) ? GetContactNameWithEmail(GetConvertetString(clearSenderName), clearsenderEmail) : GetContactName(GetConvertetString(clearSenderName)));
            if (!string.IsNullOrEmpty(mail.CC))
                page += string.Format(CCRow, HighlightSearchString(GetConvertetString(mail.CC)));
            page += string.Format(ToRow, GetRecipientsRow(mail));
            var folder = GetEmailFolder();
            if (folder != null && !string.IsNullOrEmpty(folder.Item1) && !string.IsNullOrEmpty(folder.Item2))
            {
                page += string.Format(InRow, folder.Item1, folder.Item2);
            }
            page += string.Format(SendRow, mail.ReceivedTime.ToString());
            page += GetAttachments(mail, filename);
            string temp = GetHtmlBodyHightlight(mail.HTMLBody);
            page += string.Format(EmailRow, temp);
            page += TableEnd + PageEnd;
            return page;
        }

        public string GetPreviewForEmail(EmailSearchObject email)
        {
            string page = GetBeginingOfPreview(email);
            var senderEmailAddress = email.FromAddress;
            var clearSenderName = email.FromName.ClearString();
            var clearsenderEmail = senderEmailAddress.ClearString();
            page += string.Format(SenderRow, IsEmail(senderEmailAddress) ? GetContactNameWithEmail(GetConvertetString(clearSenderName), clearsenderEmail) : GetContactName(GetConvertetString(clearSenderName)));
            if (email.Cc != null && email.Cc.Length > 0)
                page += string.Format(CCRow, HighlightSearchString(GetRecipientsRow(email.Cc)));
            page += string.Format(ToRow, GetRecipientsRow(email.To));
            var folder = GetEmailFolder();
            if (folder != null && !string.IsNullOrEmpty(folder.Item1) && !string.IsNullOrEmpty(folder.Item2))
            {
                page += string.Format(InRow, folder.Item1, folder.Item2);
            }
            page += string.Format(SendRow, email.DateReceived.ToString());
            if(email.IsAttachmentPresent)
                page += GetAttachments(email);
            string temp = GetHtmlBodyHightlight(email.HtmlContent);
            page += string.Format(EmailRow, temp);
            page += TableEnd + PageEnd;
            return page;
        }


        private string GetRecipientsRow(Outlook.MailItem mail)
        {
            if (mail == null || mail.Recipients == null || mail.Recipients.Count == 0)
            {
                return string.Empty;
            }

            var list = new List<string>();
            foreach (Outlook.Recipient recipient in mail.Recipients.OfType<Outlook.Recipient>())
            {
                var clearStr = recipient.Name.ClearString();
                var email = recipient.GetSMTPAddress();
                var tt = IsEmail(email) ? GetContactNameWithEmail(clearStr, email) : GetContactName(clearStr);
                list.Add(tt);
            }
            return string.Join("; ", list.Where(s => !string.IsNullOrEmpty(s)));
        }

        private string GetRecipientsRow(OFRecipient[] ccs)
        {
            if (ccs == null || ccs.Length == 0)
            {
                return string.Empty;
            }
            var list = new List<string>();
            foreach (OFRecipient recipient in ccs)
            {
                var clearStr = recipient.Name.ClearString();
                var emailAddress = recipient.Address;
                var tt = IsEmail(emailAddress) ? GetContactNameWithEmail(clearStr, emailAddress) : GetContactName(clearStr);
                list.Add(tt);
            }
            return string.Join("; ", list.Where(s => !string.IsNullOrEmpty(s)));
        }

        private string GetConvertetString(string str)
        {
            return !string.IsNullOrEmpty(str) ? str.DecodeString() : NAEmpty;
        }

        public string GetPreviewForAppointment(Outlook.AppointmentItem appointment, string filename)
        {
            string page = GetBeginingOfPreview(appointment);

            page += string.Format(StartRow, appointment.Start.ToString());
            page += string.Format(EndRow, appointment.End.ToString());
            page += string.Format(LocationRow, appointment.Location);
            page += string.Format(RequiredRow, GetMailTo(appointment.RequiredAttendees.Split(';')));
            page += GetAttachments(appointment, filename);
            //page += string.Format(EmailRow, HighlightSearchString(appointment.Body));

            page += TableEnd + PageEnd;

            return page;
        }

        private string GetMailTo(string[] mails)
        {
            if (mails == null || mails.Length == 0)
                return string.Empty;
            string mailtostring = string.Empty;
            foreach (var mail in mails)
            {
                if (IsEmail(mail))
                    mailtostring += string.Format(MailTo, mail);
                else
                    mailtostring += string.Format("{0}; ", mail);
            }
            return mailtostring;
        }

        private string GetMailTo(string name, string email)
        {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(email) && IsEmail(email))
            {
                return string.Format(MailToWithName, email, name);
            }
            if (!string.IsNullOrEmpty(name) && (string.IsNullOrEmpty(email) || !IsEmail(email)))
            {
                return string.Format(MailToWithName, name, name);
            }
            if (string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(email) && IsEmail(email))
            {
                return string.Format(MailTo, email);
            }
            return string.Empty;
        }

        private string GetContactNameWithEmail(string name, string email)
        {
            return string.Format(ContactNameWithEmail, string.Format("{0}:{1}", name, email), HighlightSearchString(name));
        }

        private string GetContactName(string name)
        {
            return string.Format(ContactName, name);
        }

        private bool IsEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            return regex.IsMatch(email);
        }

        public string GetPreviewForMeeting(Outlook.MeetingItem meeting, string filename)
        {
            string page = GetBeginingOfPreview(meeting);
            page += string.Format(TopicRow, meeting.ConversationTopic.DecodeString());
            page += string.Format(SendRow, GetMailTo(new string[] { meeting.SenderName.DecodeString() }));
            page += GetAttachments(meeting, filename);
            page += TableEnd + PageEnd;
            return page;
        }

        private string GetAttachmentValue(Outlook.Attachment att, string tempFolder)
        {
            List<string> resourceArray = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(s => s.EndsWith(ExtOfImage)).ToList();
            var ext = Path.GetExtension(att.FileName);
            ext = ext.Substring(1, ext.Length - 1);
            var key = string.Format("{0}.{1}", ext, ExtOfImage);
            var name = resourceArray.FirstOrDefault(s => s.EndsWith(key));
            if (string.IsNullOrEmpty(name))
            {
                return ReturnKeyImage(resourceArray, "_blank", tempFolder);
            }
            return ReturnKeyImage(resourceArray, ext, tempFolder);
        }

        private string ReturnKeyImage(List<string> keysList, string key, string tempFolder)
        {
            var k = string.Format("{0}.{1}", key, ExtOfImage);
            var name = keysList.FirstOrDefault(s => s.EndsWith(k));
            string destname = string.Empty;
            if (!_dictImage.ContainsKey(name))
            {
                var imagename = name.Substring(name.IndexOf(string.Format("{0}.{1}", key, ExtOfImage)));
                destname = string.Format("{0}\\{1}", tempFolder, imagename);
                Copy(name, destname);
                _dictImage.Add(name, destname);
            }
            else
            {
                destname = _dictImage[name];
            }
            return destname;
        }

        private bool Copy(string sourceName, string destFileName)
        {
            bool res = false;
            string saveAsName = destFileName;
            FileInfo fileInfoOutputFile = new FileInfo(saveAsName);

            if (fileInfoOutputFile.Exists)
            {
            }

            using (FileStream streamToOutputFile = fileInfoOutputFile.OpenWrite())
            using (Stream streamToResourceFile = Assembly.GetExecutingAssembly().GetManifestResourceStream(sourceName))
            {
                const int size = 4096;
                byte[] bytes = new byte[4096];
                int numBytes;
                while ((numBytes = streamToResourceFile.Read(bytes, 0, size)) > 0)
                {
                    streamToOutputFile.Write(bytes, 0, numBytes);
                }
                res = true;
            }

            return res;
        }

        private Tuple<string, string> GetEmailFolder()
        {
            if (string.IsNullOrEmpty(FullFolderPath))
                return default(Tuple<string, string>);
            var name = FullFolderPath.Substring(FullFolderPath.LastIndexOf('\\') + 1);
            return new Tuple<string, string>(FullFolderPath, name);
        }

        #endregion [generate privew]
    }
}