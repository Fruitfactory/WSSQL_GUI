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
    [View(typeof(ContactSettingsTask),ContactSettingsTask.ContactSettingsView)]
    public partial class ContactSettingsView : BaseSettingsView,IContactSettingsView
    {
        public ContactSettingsView()
        {
            InitializeComponent();
        }

        public override IController Controller
        {
            get
            {
                return base.Controller as ContactSettingsController;
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
            if(Controller == null)
                return;
            var contactController = Controller as ContactSettingsController;
            if (contactController == null)
                return;
            if (buttonSearch.DataBindings.Count == 0)
                commandManager.Bind(contactController.SearchCommand, buttonSearch);
            contactController.Suggest += (o, e) => Invoke(new Action<List<string>>(OnSuggest), new object[] { e.Value });
            textSearchComplete.TextChanging +=
                (o, e) => contactController.StartSuggesting(textSearchComplete.Text);
            comboBoxFolder.DataSource = contactController.GetFolders();

        }

        public string SearchCriteria
        {
            get { return textSearchComplete.Text; }
            set { textSearchComplete.Text = value; }
        }

        private void OnSuggest(List<string> listSuggest )
        {
            textSearchComplete.DataSource = listSuggest;
        }


        public string FolderContact
        {
            get { return comboBoxFolder.SelectedValue.ToString(); }
            set{}
        }
    }
}
