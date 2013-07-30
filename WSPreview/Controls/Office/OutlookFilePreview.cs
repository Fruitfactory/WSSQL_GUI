﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using WSPreview.PreviewHandler.PreviewHandlerFramework;
using WSPreview.PreviewHandler.Service;
using WSPreview.PreviewHandler.Service.Logger;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Text.RegularExpressions;
using mshtml;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace WSPreview.PreviewHandler.Controls.Office
{
    [KeyControl(ControlsKey.Outlook)]
    public partial class OutlookFilePreview : UserControl,IPreviewControl
    {
        private const string AfterStrongTemplate = "<font style='background-color: yellow'><strong>{0}</strong></font>";
        private const string OutlookProcessName = "OUTLOOK";
        private const string OutlookApplication = "Outlook.Application";
        private const string WordRegex = @"(?<word>\w+)";
        private const string ExtOfImage = "png";

        #region pattern for html

        private const string PageBegin = @"<!DOCTYPE html><html lang='en'><head><title></title><meta charset='utf-8' http-equiv='Content-Type' content='text/html; charset=utf-8;'><style type='text/css'>.style1{width: 15%;color: gray;}.style2{width: 85%;}</style></head><body style='font-family: Arial, Helvetica, sans-serif'>";
        private const string TableBegin = @"<table style='width: 100%; table-layout: auto;'>";
        private const string SubjectRow = @"<tr><td class='style1' style='color: #0c0202; font-size: large; margin: 5px 5px 5px 5px' colspan='2'>{0}</td></tr>";
        private const string SenderRow = @"<tr><td class='style1' style='color: #0c0202; font-size: medium; margin: 5px 5px 5px 5px' colspan='2'>{0}</td></tr>";
        private const string ToRow = @"<tr><td class='style1'>To:</td><td class='style2'>{0}</td></tr>";
        private const string CCRow = @"<tr><td class='style1'>CC:</td><td class='style2'>{0}</td></tr>";
        private const string AttachmentsRow = @"<tr><td class='style1'>Attachments:</td><td class='style2'>{0}</td></tr>";
        private const string SendRow = @"<tr><td class='style1'>Send:</td><td class='style2'>{0}</td></tr>";
        
        private const string EmailRow = @"<tr style='margin: 25px 10px 10px 10px'><td colspan='2' ><hr /><html><body>{0}</body></html></td></tr>";
        private const string TableEnd = @"</table>";
        private const string PageEnd = @"</body></html>";

        //appointment
        private const string StartRow = @"<tr><td class='style1'>Start:</td><td class='style2'>{0}</td></tr>";
        private const string EndRow = @"<tr><td class='style1'>End:</td><td class='style2'>{0}</td></tr>";
        private const string RequiredRow = @"<tr><td class='style1'>Required Attendees:</td><td class='style2'>{0}</td></tr>";
        private const string LocationRow = @"<tr><td class='style1'>Location: </td><td class='style2'><a href='{0}'>{0}</a></td></tr>";
        private const string MailTo = @"<a href='mailto:{0}'>{0}</a> "; 

        // meeting
        private const string TopicRow = @"<tr><td class='style1'>Topic:</td><td class='style2'>{0}</td></tr>";


        private const string LinkTemplate = @"<img src='{1}' width='16' height='16' /><a href='{0}'>{2}</a>&nbsp;&nbsp;&nbsp;";

        private const string HtmlTagOpenName = "html";
        private const string HtmlTagcloseName = "/html";

        private const string HtmlTemplate = "<html xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='rn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-microsoft-com:office:word' xmlns:m='http://schemas.microsoft.com/office/2004/12/omml' xmlns='http://www.w3.org/TR/REC-html40'><head></head><body>{0}</body></html>";



        #endregion

        private bool _IsExistProcess = false;
        private string _filename = string.Empty;
        private readonly Dictionary<string,string> _dictTempFile = new Dictionary<string, string>();
        private readonly Dictionary<string,string> _dictImage = new Dictionary<string, string>();
        private string _hitString;
        private string[] _itemArray = null;

        private const int WM_KEYDOWN = 0x0100;

        public OutlookFilePreview()
        {
            InitializeComponent();
            //webEmail.Navigating += (sender, args) => OnNavigating(args);
            webEmail.BeforeNavigate += WebEmailOnBeforeNavigate;
        }

        #region public 

        public string HitString 
        { 
            get{return _hitString;}
            set 
            { 
                _hitString = value;
                _itemArray = GetWordsList(_hitString);
            } 
        }

        public void LoadFile(string filename)
        {
            _filename = filename;
            Outlook.Application app = GetApplication();
            if (app == null)
                return;

            //Outlook.MailItem mail = (Outlook.MailItem)app.CreateItemFromTemplate(filename);
            var mail = app.CreateItemFromTemplate(filename);

            if (mail == null)
                return;

            string page = string.Empty;

            if (mail is Outlook.MailItem)
            {
                page = GetPreviewForEmail(mail as Outlook.MailItem,filename);
            }
            else if (mail is Outlook.AppointmentItem)
            {
                page = GetPreviewForAppointment(mail as Outlook.AppointmentItem,filename);
            }
            else if (mail is Outlook.MeetingItem)
            {
                page = GetPreviewForMeeting(mail as Outlook.MeetingItem, filename);
            }

            webEmail.DocumentText = page;

            Marshal.ReleaseComObject(mail);
            CloseApplication(app);

        }

        public void LoadFile(Stream stream)
        {
        }

        public void Clear()
        {
            if (webEmail != null)
            {
                webEmail.DocumentText = string.Empty;
            }
            Unload();
        }

        public void Unload()
        {
            DeleteAllTempFile();
        }

        #endregion


        #region private

        private Outlook.Application GetApplication()
        {
            Outlook.Application ret = GetFromProcess();
            if (ret == null)
            {
                ret = CreateOutlookApplication();
            }

            return ret;
        }

        private Outlook.Application GetFromProcess()
        {
            Outlook.Application ret = null;
            try
            {
                var outlook = Process.GetProcesses().Where(p => p.ProcessName.ToUpper().StartsWith(OutlookProcessName));
                if (outlook.Count() > 0)
                {
                    ret = Marshal.GetActiveObject(OutlookApplication) as Outlook.Application;
                    _IsExistProcess = true;
                }
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError("GetFromProcess " + ex.Message);
            }

            return ret;
        }

        private Outlook.Application CreateOutlookApplication()
        {

            Outlook.Application ret = null;
            try
            {
                ret = new Outlook.Application();
                if (ret == null)
                    return ret;
                Outlook.NameSpace ns = ret.GetNamespace("MAPI");
                ns.Logon(ret.DefaultProfileName, "", Type.Missing, Type.Missing);//ret.DefaultProfileName
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError("CreateOutlookApplication" + ex.Message);
            }

            return ret;
        }

        private void CloseApplication(Outlook.Application app)
        {
            if (!_IsExistProcess)
            {
                try
                {
                    app.Quit();
                    Marshal.ReleaseComObject(app);
                }
                catch (Exception ex)
                {
                    WSSqlLogger.Instance.LogError("CloseApplication " + ex.Message);
                }
                
            }
        }
        
        public static string[] GetWordsList(string inputSequence)
        {
            if (string.IsNullOrEmpty(inputSequence))
                return new string[] { inputSequence };

            MatchCollection col = Regex.Matches(inputSequence, WordRegex);
            if (col.Count == 0)
                return new string[] { inputSequence };

            var list = new List<string>();
            for (int i = 0; i < col.Count; i++)
            {
                var item = col[i];
                if (string.IsNullOrEmpty(item.Value))
                    continue;
                list.Add(item.Value);
            }

            return list.ToArray();
        }

        private string HighlightSearchString(string inputString)
        {
            if (string.IsNullOrEmpty(HitString) || string.IsNullOrEmpty(inputString) || _itemArray == null )
                return inputString;
            string result = inputString;
            try
            {
                foreach (var s in _itemArray)
                {
                    result = Regex.Replace(result, string.Format(@"\b({0})\b", Regex.Escape(s)), string.Format(AfterStrongTemplate, s), RegexOptions.IgnoreCase);
                }
                return result;
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError("HighlightSearchString " + ex.Message);
            }
            return string.Empty;
        }

        private dynamic GetMail(Outlook.Application app,string filename)
        {
            try
            {
                dynamic mail = app.CreateItemFromTemplate(filename);

                if (mail == null)
                    return null;
                return mail;
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError("GetMail " + ex.Message);
            }
            return null;
        }

        private string GetTempFileName(string filename)
        {
            if (_dictTempFile.ContainsKey(filename))
                return _dictTempFile[filename];
            string path = string.Format("{0}{1}", Path.GetTempPath(), filename);
            _dictTempFile.Add(filename,path);
            return path;
        }

        private void DeleteAllTempFile()
        {
            if (_dictTempFile.Count == 0)
                return;

            foreach (var pair in _dictTempFile)
            {
                if(File.Exists(pair.Value))
                    File.Delete(pair.Value);
            }
            _dictTempFile.Clear();
        }

        private string GetBeginingOfPreview(dynamic item, string filename)
        {
            string page = PageBegin + TableBegin;
            page += string.Format(SubjectRow, HighlightSearchString(item.Subject));
            return page;
        }

        private string GetAttachments(dynamic item, string filename)
        {
            try
            {
                if (item.Attachments.Count > 0)
                {
                    var tempFolder = Path.GetDirectoryName(filename);

                    var urls = string.Empty;
                    foreach (Outlook.Attachment att in item.Attachments)
                    {
                        // TODO add fynctionality, if we don't have image for samo file ext (need to show blank image)
                        var destname = GetAttachmentValue(att, tempFolder);
                        if (!string.IsNullOrEmpty(destname))
                            urls += string.Format(LinkTemplate, att.DisplayName, destname, HighlightSearchString(att.DisplayName));
                    }
                    return string.Format(AttachmentsRow, urls);
                }
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError("GetAttachments " + ex.Message);
            }
            return null;
        }

        private string GetPreviewForEmail(Outlook.MailItem mail, string filename)
        {
            string page = GetBeginingOfPreview(mail, filename);

            page += string.Format(SenderRow, HighlightSearchString(mail.SenderName));
            if (!string.IsNullOrEmpty(mail.CC))
                page += string.Format(CCRow, HighlightSearchString(mail.CC));
            page += string.Format(ToRow, HighlightSearchString(mail.To));
            page += string.Format(SendRow, mail.ReceivedTime.ToString());
            page += GetAttachments(mail, filename);
            string temp = GetHtmlBodyHightlight(mail.HTMLBody);
            page += string.Format(EmailRow, temp);
            page += TableEnd + PageEnd;
            return page;
        }

        private string GetHtmlBodyHightlight(string body)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                HtmlDocument htmlDoc = new HtmlDocument();

                htmlDoc.LoadHtml(body);
                HightlightAllNodes(htmlDoc.DocumentNode);
                htmlDoc.Save(stream,Encoding.UTF8);
                stream.Position = 0;
                using(StreamReader reader = new StreamReader(stream,Encoding.UTF8))
                {
                    return reader.ReadToEnd();        
                }
            }
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
                HightlightAllNodes(childNode);
            }
        }


        private string GetPreviewForAppointment(Outlook.AppointmentItem appointment, string filename)
        {
            string page = GetBeginingOfPreview(appointment, filename);

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

        private bool IsEmail(string email)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            return regex.IsMatch(email);
        }

        private string GetPreviewForMeeting(Outlook.MeetingItem meeting,string filename)
        {
            string page = GetBeginingOfPreview(meeting, filename);
            page += string.Format(TopicRow, meeting.ConversationTopic);
            page += string.Format(SendRow, GetMailTo(new string[]{meeting.SenderName}));
            page += GetAttachments(meeting, filename);
            page += TableEnd + PageEnd;
            return page;
        }

        #endregion

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
            return ReturnKeyImage(resourceArray,ext,tempFolder);
        }

        private string ReturnKeyImage(List<string> keysList, string key,string tempFolder)
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
            
            using(FileStream streamToOutputFile = fileInfoOutputFile.OpenWrite())
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

        private void OnNavigating(WebBrowserNavigatingEventArgs args)
        {
            string path = string.Empty;
            switch (args.Url.Scheme)
            {
                case "https":
                case "mailto":
                case "http":
                    path = args.Url.AbsoluteUri;
                    break;
                case "about":
                    path = GetPathForEmail(args.Url);
                    break;
            }

            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    args.Cancel = true;
                    Process.Start(path);
                }
                catch (Exception ex)
                {
                    WSSqlLogger.Instance.LogError(ex.Message);
                }
            }
            else
            {
                WSSqlLogger.Instance.LogInfo("Path is empty. Outlook Preview");
            }
           
            
        }

        private string GetPathForEmail(Uri url)
        {
            Outlook.Application app = GetApplication();
            if (app == null)
                return string.Empty;
            dynamic mail = GetMail(app, _filename);
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
            CloseApplication(app);
            return File.Exists(path) ? path : string.Empty;
        }

        public void CopySelectedText()
        {
            IHTMLDocument2 htmlDocument = webEmail.Document.DomDocument as IHTMLDocument2;
            IHTMLSelectionObject currentSelection = htmlDocument.selection;
            if (currentSelection != null)
            {
                IHTMLTxtRange range = currentSelection.createRange() as IHTMLTxtRange;
                if (range != null && range.text  != null)
                {
                    Clipboard.SetText(range.text);
                }
            }
        }

        private void WebEmailOnBeforeNavigate(object sender, WebBrowserNavigatingEventArgs args)
        {
            string path = string.Empty;
            switch (args.Url.Scheme)
            {
                case "https":
                case "mailto":
                case "http":
                    path = args.Url.AbsoluteUri;
                    break;
                case "about":
                case "re":
                    path = GetPathForEmail(args.Url);
                    break;
            }

            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    args.Cancel = true;
                    Process.Start(path);
                }
                catch (Exception ex)
                {
                    WSSqlLogger.Instance.LogError(ex.Message);
                }
            }
            else
            {
                WSSqlLogger.Instance.LogInfo("Path is empty. Outlook Preview");
            }
        }

    }

}
