using System;
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
using C4F.DevKit.PreviewHandler.Service.Logger;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Text.RegularExpressions;

namespace C4F.DevKit.PreviewHandler.Controls.Office
{
    public partial class OutlookFilePreview : UserControl
    {
        private const string AfterStrongTemplate = "<font style='background-color: yellow'><strong>{0}</strong></font>";
        private const string OutlookProcessName = "OUTLOOK";
        private const string OutlookApplication = "Outlook.Application";
        private const string WordRegex = @"(?<word>\w+)";
        private const string ExtOfImage = "png";

        #region pattern for html

        private const string PageBegin = @"<html><head><title></title><style type='text/css'>.style1{width: 15%;color: gray;}.style2{width: 85%;}</style></head><body style='font-family: Arial, Helvetica, sans-serif'>";
        private const string TableBegin = @"<table style='width: 100%; table-layout: auto;'>";
        private const string SubjectRow = @"<tr><td class='style1' style='color: #0c0202; font-size: large; margin: 5px 5px 5px 5px' colspan='2'>{0}</td></tr>";
        private const string SenderRow = @"<tr><td class='style1' style='color: #0c0202; font-size: medium; margin: 5px 5px 5px 5px' colspan='2'>{0}</td></tr>";
        private const string ToRow = @"<tr><td class='style1'>To:</td><td class='style2'>{0}</td></tr>";
        private const string CCRow = @"<tr><td class='style1'>CC:</td><td class='style2'>{0}</td></tr>";
        private const string AttachmentsRow = @"<tr><td class='style1'>Attachments:</td><td class='style2'>{0}</td></tr>";
        private const string SendRow = @"<tr><td class='style1'>Send:</td><td class='style2'>{0}</td></tr>";
        private const string EmailRow = @"<tr style='margin: 25px 10px 10px 10px'><td colspan='2' ><hr />{0}</td></tr>";
        private const string TableEnd = @"</table>";
        private const string PageEnd = @"</body></html>";

        private const string LinkTemplate = @"<img src='{1}' width='16' height='16' /><a href='{0}'>{0}</a>&nbsp;&nbsp;&nbsp;";

        #endregion

        private bool _IsExistProcess = false;
        private string _filename = string.Empty;
        private readonly Dictionary<string,string> _dictTempFile = new Dictionary<string, string>();
        private readonly Dictionary<string,string> _dictImage = new Dictionary<string, string>(); 



        public OutlookFilePreview()
        {
            InitializeComponent();
            webEmail.Navigating += (sender, args) => OnNavigating(args);
        }


        #region public 

        public string HitString { get; set; }

        public void LoadFile(string filename)
        {
            _filename = filename;
            Outlook.Application app = GetApplication();
            if (app == null)
                return;

            Outlook.MailItem mail = (Outlook.MailItem)app.CreateItemFromTemplate(filename);

            if (mail == null)
                return;
           
            string page = PageBegin + TableBegin;

            page += string.Format(SubjectRow, mail.Subject);
            page += string.Format(SenderRow, mail.SenderName);
            page += string.Format(ToRow, mail.To);
            if (!string.IsNullOrEmpty(mail.CC))
                page += string.Format(CCRow, mail.CC);

            if (mail.Attachments.Count > 0)
            {
                var tempFolder = Path.GetDirectoryName(filename);
                
                var urls = string.Empty;
                foreach (Outlook.Attachment att in mail.Attachments)
                {
                    // TODO add fynctionality, if we don't have image for samo file ext (need to show blank image)
                    var destname = GetAttachmentValue(att, tempFolder);
                    if(!string.IsNullOrEmpty(destname))
                        urls += string.Format(LinkTemplate, att.DisplayName,destname);
                }
                page += string.Format(AttachmentsRow, urls);
            }

            page += string.Format(SendRow, mail.ReceivedTime.ToString());
            page += string.Format(EmailRow, mail.HTMLBody);


            page += TableEnd + PageEnd;

            webEmail.DocumentText = HighlightSearchString(page);


            Marshal.ReleaseComObject(mail);
            CloseApplication(app);


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
                WSSqlLogger.Instance.LogError(ex.Message);
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
                WSSqlLogger.Instance.LogError(ex.Message);
            }

            return ret;
        }

        private void CloseApplication(Outlook.Application app)
        {
            if (!_IsExistProcess)
            {
                app.Quit();
                Marshal.ReleaseComObject(app);
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
            if (HitString == null)
                return inputString;
            string result = inputString;
            var itemArray = GetWordsList(HitString.Trim());
            foreach (var s in itemArray)
            {
                result = Regex.Replace(result, string.Format(@"\b({0})\b",Regex.Escape(s)), string.Format(AfterStrongTemplate, s), RegexOptions.IgnoreCase);
            }
            return result;
        }

        private Outlook.MailItem GetMail(Outlook.Application app,string filename)
        {
            Outlook.MailItem mail = (Outlook.MailItem)app.CreateItemFromTemplate(filename);

            if (mail == null)
                return null;
            return mail;
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

        #endregion

        private string GetAttachmentValue(Outlook.Attachment att, string tempFolder)
        {
            List<string> resourceArray = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(s => s.EndsWith(ExtOfImage)).ToList();
            var ext = Path.GetExtension(att.DisplayName);
            ext = ext.Substring(1, ext.Length - 1);
            var key = string.Format("{0}.{1}", ext, ExtOfImage);
            var name = resourceArray.FirstOrDefault(s => s.EndsWith(key));
            if (string.IsNullOrEmpty(name))
                return string.Empty;
            string destname = string.Empty;
            if (!_dictImage.ContainsKey(name))
            {
                var imagename = name.Substring(name.IndexOf(string.Format("{0}.{1}", ext, ExtOfImage)));
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
            
            Outlook.Application app = GetApplication();
            if (app == null)
                return ;
            Outlook.MailItem mail = GetMail(app, _filename);
            if(mail == null)
                return;
            
            if (mail.Attachments.Count == 0)
                return;
            string currentname = args.Url.LocalPath;
            
            string path = string.Empty;
            foreach (Outlook.Attachment att in mail.Attachments)
            {
                if (att.DisplayName == currentname)
                {
                    path = GetTempFileName(currentname);
                    att.SaveAsFile(path);
                    break;
                }
            }
            if (File.Exists(path))
                try
                {
                    args.Cancel = true;
                    Process.Start(path);
                }
                catch (Exception ex)
                {
                    WSSqlLogger.Instance.LogError(ex.Message);
                }
            Marshal.ReleaseComObject(mail);
            CloseApplication(app);
            
        }



    }

}
