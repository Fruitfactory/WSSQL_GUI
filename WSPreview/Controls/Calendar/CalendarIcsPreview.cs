using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WSPreview.PreviewHandler.PreviewHandlerFramework;

namespace WSPreview.PreviewHandler.Controls.Calendar
{
    public partial class CalendarIcsPreview : UserControl, IPreviewControl
    {

        private const string AfterStrongTemplate = "<font style='background-color: yellow'><strong>{0}</strong></font>";
        private const string WordRegex = @"(?<word>\w+)";

        private const string UrlPAttern =
            @"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?";

        #region pattern for html

        private const string PageBegin = @"<html><head><title></title><style type='text/css'>.style1{width: 15%;color: gray;}.style2{width: 85%;}</style></head><body style='font-family: Arial, Helvetica, sans-serif'>";
        private const string TableBegin = @"<table style='width: 100%; table-layout: auto;'>";
        private const string Row = @"<tr><td class='style1'>{1}:</td><td class='style2'>{0}</td></tr>";
        private const string TableEnd = @"</table>";
        private const string PageEnd = @"</body></html>";
        private const string MailTo = @"<a href='mailto:{0}'>{0}</a>";
        private const string LinkTemplate = @"<img src='{1}' width='16' height='16' /><a href='{0}'>{2}</a>&nbsp;&nbsp;&nbsp;";
        private const string ALink = "<a href='{0}'>{0}</a>";

        #endregion


        private string _filename = string.Empty;

        public CalendarIcsPreview()
        {
            InitializeComponent();
        }


        public void LoadFile(string filename)
        {
            _filename = filename;

            if(string.IsNullOrEmpty(_filename))
                return;
            StreamReader  file = null;
            try
            {
                file = File.OpenText(_filename);
                string content = file.ReadToEnd();
                FileIcsProperty obj = new FileIcsProperty();
                ParserIcs.ParseContent(content,obj);
                browserIcs.DocumentText = GeneratePreview(obj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (file != null)
                    file.Close();
            }
        }

        public void LoadFile(Stream stream)
        {
        }

        private string GeneratePreview(FileIcsProperty obj)
        {
            string page = string.Empty;
            page += PageBegin + TableBegin;
            page += string.Format(Row, obj.Organizer, GetDisplayName(() => obj.Organizer));
            page += string.Format(Row, string.Format(MailTo,obj.MailTo), GetDisplayName(() => obj.MailTo));
            page += string.Format(Row, obj.DateStart, GetDisplayName(() => obj.DateStart));
            page += string.Format(Row, obj.DateEnd, GetDisplayName(() => obj.DateEnd));
            page += string.Format(Row, obj.DateCreated, GetDisplayName(() => obj.DateCreated));
            page += string.Format(Row, ParseDescription(obj.Description), GetDisplayName(() => obj.Description));
            page += string.Format(Row, obj.Status, GetDisplayName(() => obj.Status));
            page += string.Format(Row, obj.Summary, GetDisplayName(() => obj.Summary));
            page += TableEnd + PageEnd;

            return page;
        }

        private string GetDisplayName<T>(Expression<Func<T>> expression)
        {
            var expr = (MemberExpression)expression.Body;
            var attr = (ParseAttribute[])expr.Member.GetCustomAttributes(typeof(ParseAttribute),false);

            return attr.Length > 0 ? attr[0].Display : expr.Member.Name;
        }

        private string ParseDescription(string desc)
        {
            string res = desc;
            var matches = Regex.Matches(desc, UrlPAttern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Dictionary<string,string> dictLink = new Dictionary<string, string>();
            if (matches.Count > 0)
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    var match = matches[i];
                    string temp = desc.Substring(match.Index, match.Length);
                    dictLink.Add(temp, string.Format(ALink, temp));
                }
                foreach (var keyPair in dictLink)
                {
                    res = res.Replace(keyPair.Key, keyPair.Value);
                }
            }
            return res;
        }

    }
}
