using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Documents;
using System.Windows.Forms;
using WSUI.Core.Logger;

namespace WSUIOutlookPlugin.About
{
    public partial class WSUIAbout : Form
    {
        private const string VersionTemplate = "Version: v{0}.{1}.{2}.{3}";
        private const string CompanyTemplate = "Company: {0}";
        private const string EmailTemplate = "Email: {0}";
        private const string DescriptionTemplate = "Desription: {0}";
        

        public WSUIAbout()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var currentAssembly = typeof (WSUIAbout).Assembly;
            if (currentAssembly == null)
                return;
            var title = (AssemblyTitleAttribute[])currentAssembly.GetCustomAttributes(typeof (AssemblyTitleAttribute), true);
            var copyright = (AssemblyCopyrightAttribute[])currentAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);
            var company = (AssemblyCompanyAttribute[])currentAssembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
            var description = (AssemblyDescriptionAttribute[])currentAssembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), true);
            if (title != null && title.Length > 0)
            {
                labelProductName.Text = title[0].Title;
            }
            if (copyright != null && copyright.Length > 0)
            {
                labelCopyright.Text = copyright[0].Copyright;
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.AllRights))
            {
                labelRights.Text = Properties.Settings.Default.AllRights;
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.HelpUrl))
            {
                linkSupport.Text = Properties.Settings.Default.HelpUrl;
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.ProductUrl))
            {
                linkSite.Text = Properties.Settings.Default.ProductUrl;
            }
            var strBuilder = new StringBuilder();
            var version = currentAssembly.GetName().Version;
            WSSqlLogger.Instance.LogInfo("Version: {0}",version);
            if(version != null)
                strBuilder.AppendLine(string.Format(VersionTemplate, version.Major,version.Minor,version.Build,version.Revision));
            if (description != null && description.Length > 0)
            {
                strBuilder.AppendLine(string.Format(DescriptionTemplate, description[0].Description));
            }
            if (company != null && company.Length > 0)
            {
                strBuilder.AppendLine(string.Format(CompanyTemplate, company[0].Company));
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Email))
            {
                strBuilder.AppendLine(string.Format(EmailTemplate, Properties.Settings.Default.Email));
            }
            textBoxInfo.Text = strBuilder.ToString();
        }

        private void linkSupport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RunProcess(((LinkLabel)sender).Text);
        }

        private void RunProcess(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError("Run url: {0}",ex.Message);
            }
        }

        private void linkSite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RunProcess(((LinkLabel)sender).Text);
        }

    }
}
