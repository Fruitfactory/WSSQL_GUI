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
using MVCSharp.Core.Configuration.Views;

namespace WSSQLGUI.Views
{
    [View(typeof(AllFilesSettingsTask),AllFilesSettingsTask.AllFilesSettingsView)]
    internal partial class AllFilesSettingsView : BaseSettingsView, IAllFilesSettingsView
    {
        public AllFilesSettingsView()
        {
            InitializeComponent();
            textBoxSearch.TextChanged += (o, e) => SearchCriteriaValidate();
            textBoxSearch.Validated += (o, e) => SearchCriteriaValidate();
        }

        protected override void OnInit()
        {
            tabControlBase.TabPages.RemoveAt(1);
        }


        public override IController Controller
        {
            get
            {
                return  base.Controller as AllFilesSettingsController; 
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
            if (Controller == null)
                return;
            if(buttonSearch.DataBindings.Count == 0)
                commandManager.Bind((Controller as AllFilesSettingsController).SearchCommand, buttonSearch);
        }

        public string SearchCriteria
        {
            get
            {
                return  textBoxSearch.Text;
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
            var message = (Controller as AllFilesSettingsController).ErrorProvider(textBoxSearch.Text);
            errorProvider.SetError(textBoxSearch, message);
        }

    }
}
