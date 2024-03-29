﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MVCSharp.Core;
using MVCSharp.Winforms;
using WSSQLGUI.Controllers;
using MVCSharp.Core.Configuration.Views;
using WSSQLGUI.Controllers.Tasks;
using WSSQLGUI.Core;
using WSSQLGUI.Services;

namespace WSSQLGUI.Views
{
    [View(typeof(EmailSettingsTask),EmailSettingsTask.EmailSettingsView)]
	internal partial class EmailSettingsView: BaseSettingsView,IEmailSettingsView
	{
		public EmailSettingsView()
		{
			InitializeComponent();
            textBoxSearch.TextChanged += (o, e) => SearchCriteriaValidate();
            textBoxSearch.Validated += (o, e) => SearchCriteriaValidate();
		}

        public override IController Controller
        {
            get
            {
                return base.Controller as EmailSettingsController;
            }
            set
            {
                if(value != null)
                    base.Controller = value;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            var contr = Controller as EmailSettingsController;
            if (contr == null)
                return;
            if(buttonSearch.DataBindings.Count == 0)
            	commandManager.Bind((contr as IBaseSettingsController).SearchCommand, buttonSearch);
            comboBoxFolder.DataSource = (contr as IEmailSettings).GetFolders();
            #region ti4ka
            int index = -1;
            if((index = comboBoxFolder.Items.IndexOf(HelperConst.Inbox1)) > -1)
            {
                comboBoxFolder.SelectedIndex = index;
            }
            else if((index = comboBoxFolder.Items.IndexOf(HelperConst.Inbox2)) > -1)
            {
                comboBoxFolder.SelectedIndex = index;
            }
            #endregion
        }


        public string Folder
        {
            get
            {
                return comboBoxFolder.SelectedValue.ToString();
            }
            set
            {
                comboBoxFolder.SelectedText = value;
            }
        }

        public string SearchCriteria
        {
            get
            {
                return textBoxSearch.Text;
            }
            set
            {
                textBoxSearch.Text = value;
            }
        }

        private void SearchCriteriaValidate()
        {
            if (Controller == null)
                return;
            var message = (Controller as EmailSettingsController).ErrorProvider(textBoxSearch.Text);
            errorProvider.SetError(textBoxSearch, message);
        }

    
    }
}
