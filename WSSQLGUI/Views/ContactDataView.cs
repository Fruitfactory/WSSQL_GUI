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
using WSSQLGUI.Controllers.Tasks;
using WSSQLGUI.Core;
using MVCSharp.Core.Configuration.Views;
using WSSQLGUI.Models;
using WSSQLGUI.Services;


namespace WSSQLGUI.Views
{
    [View(typeof(ContactDataTask),ContactDataTask.ContactDataView)]
    internal partial class ContactDataView : WinUserControlView,IContactDataView
    {
        private const string NA = "<n/a>";
        private Font _defaultFont;

        public ContactDataView()
        {
            InitializeComponent();
            _defaultFont = dataGridViewContactEmails.DefaultCellStyle.Font;
        }

        public override IController Controller
        {
            get
            {
                return base.Controller as ContactDataController;
            }
            set
            {
                if(value != null)
                    base.Controller = value;
            }
        }

        public ContactSearchData CurrentContact
        {
            get; 
            private set;
        }


        public override void Initialize()
        {
            base.Initialize();
            if(Controller == null)
                return;
            commandManager.Bind((Controller as ContactDataController).NewMailCommand, linkLabelEmail);
            commandManager.Bind((Controller as ContactDataController).NewMailCommand2, linkLabelEmail2);
            commandManager.Bind((Controller as ContactDataController).NewMailCommand3, linkLabelEmail3);
        }

        public bool IsLoading
        {
            get; set; }

        public event EventHandler<Services.EventArgs<BaseSearchData>> SelectedItemChanged;

        public void SetContextMenu(ContextMenuStrip contextMenu)
        {
            
        }

        public void SetData(object[] value, BaseSearchData data)
        {
            this.Invoke(new Action(
               () =>
               {
                   int i = dataGridViewContactEmails.Rows.Add(value);
                   DataGridViewRow row = dataGridViewContactEmails.Rows[i];
                   row.Tag = data;
               }
               ));
        }

        public void Clear()
        {
            dataGridViewContactEmails.Rows.Clear();
        }

        public void SetContact(BaseSearchData data)
        {
            if (data == null)
                return;
            var contactData = data as ContactSearchData;
            Invoke(new Action(
                       () =>
                           {
                               labelName.Text = string.Format("{0} {1}", contactData.FirstName, contactData.LastName);
                               linkLabelEmail.Text = string.IsNullOrEmpty(contactData.EmailAddress) ? NA : contactData.EmailAddress;
                               linkLabelEmail2.Text = string.IsNullOrEmpty(contactData.EmailAddress2) ? NA : contactData.EmailAddress2;
                               linkLabelEmail3.Text = string.IsNullOrEmpty(contactData.EmailAddress3) ? NA : contactData.EmailAddress3;
                               if (!string.IsNullOrEmpty(contactData.Foto))
                                   pictureBox.Image = new Bitmap(contactData.Foto);
                               else
                                   pictureBox.Image = null;
                               CurrentContact = contactData;
                           }));
        }

        private void dataGridViewContactEmails_SelectionChanged(object sender, EventArgs e)
        {
            if(IsLoading || dataGridViewContactEmails.SelectedRows.Count == 0)
                return;

            EmailSearchData data = dataGridViewContactEmails.SelectedRows[0].Tag as EmailSearchData;
            EventHandler<EventArgs<BaseSearchData>> temp = SelectedItemChanged;
            if(temp != null)
                temp(this,new EventArgs<BaseSearchData>(data));

        }

        private void dataGridViewContactEmails_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            dataGridViewContactEmails.Rows[e.RowIndex].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        }

        private void dataGridViewContactEmails_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            dataGridViewContactEmails.Rows[e.RowIndex].DefaultCellStyle.Font = _defaultFont;
        }

        #region private


        #endregion





        public void StartLoading()
        {
            
        }

        public void FinishLoading()
        {
            
        }

        public void SetData(IList<BaseSearchData> listData)
        {
            
        }
    }
}
