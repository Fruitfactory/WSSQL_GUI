using System;
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

namespace WSSQLGUI.Views
{
    internal partial class AllFilesSettingsView : WinUserControlView<AllFilesSettingsController>, IAllFilesSettingsView
    {
        public AllFilesSettingsView()
        {
            InitializeComponent();
            textBoxSearch.TextChanged += (o, e) => SearchCriteriaValidate();
            textBoxSearch.Validated += (o, e) => SearchCriteriaValidate();
        }

        public override void Initialize()
        {
            base.Initialize();
            if (Controller == null)
                return;
            commandManager.Bind((Controller as AllFilesSettingsController).SearchCommand, buttonSearch);
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
            var message = (Controller as AllFilesSettingsController).ErrorProvider(textBoxSearch.Text);
            errorProvider.SetError(textBoxSearch, message);
        }

    }
}
