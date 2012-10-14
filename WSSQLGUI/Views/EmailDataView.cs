using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WSSQLGUI.Controllers;
using MVCSharp.Winforms;
using WSSQLGUI.Controllers.Tasks;
using WSSQLGUI.Models;
using WSSQLGUI.Core;
using MVCSharp.Core.Configuration.Views;
using MVCSharp.Core;
using WSSQLGUI.Services.Enums;

namespace WSSQLGUI.Views
{
    [View(typeof(EmailDataTask), EmailDataTask.EmailDataView)]
    internal partial class EmailDataView : WinUserControlView, IEmailDataView
    {
        private EmailSearchData _current;
        private Font _defaultFont;

        public EmailDataView()
        {
            InitializeComponent();
            dataGridViewEmail.SelectionChanged += DataGridSelectionChanged;
            _defaultFont = dataGridViewEmail.DefaultCellStyle.Font;
        }

        public override IController Controller
        {
            get
            {
                return base.Controller as EmailDataController;
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
        }

        public Models.EmailSearchData CurrentEmailItem
        {
            get { return _current; }
        }

        public bool IsLoading
        {
            get;
            set;
        }

        public void Clear()
        {
            dataGridViewEmail.Rows.Clear();
        }

        public event EventHandler<Services.EventArgs<Core.BaseSearchData>> SelectedItemChanged;

        public void SetContextMenu(ContextMenuStrip contextMenu)
        {
            dataGridViewEmail.ContextMenuStrip = contextMenu;
        }

        public void SetData(object[] value, Core.BaseSearchData data)
        {
            this.Invoke(new Action(
                () => 
                {
                    int i = dataGridViewEmail.Rows.Add(value);
                    DataGridViewRow row = dataGridViewEmail.Rows[i];
                    row.Tag = data;
                    DataGridViewComboBoxCell cell = row.Cells[3] as DataGridViewComboBoxCell;
                    if (cell != null && (data as EmailSearchData).Attachments.Count > 0)
                    {
                        cell.DataSource = (data as EmailSearchData).Attachments;
                        cell.Value = cell.Items[0];
                    }
                    row.Cells[4].Value = "...";
                }
                ));
        }

        private void DataGridSelectionChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            if (dataGridViewEmail.SelectedCells.Count == 0)
                return;

            EmailSearchData si = dataGridViewEmail.SelectedRows[0].Tag as EmailSearchData;
            _current = si;
            EventHandler<Services.EventArgs<BaseSearchData>> temp = SelectedItemChanged;
            if (temp != null)
            {
                temp(this, new Services.EventArgs<BaseSearchData>(si));
            }
        }

        private void dataGridViewEmail_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            dataGridViewEmail.Rows[e.RowIndex].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        }

        private void dataGridViewEmail_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            dataGridViewEmail.Rows[e.RowIndex].DefaultCellStyle.Font = _defaultFont;
        }

        private void dataGridViewEmail_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dataGridViewEmail.Rows[e.RowIndex];
            if (e.ColumnIndex != 4 || row.Cells[3].Value == null)
                return;
            EmailSearchData si = row.Tag as EmailSearchData;
            EmailSearchData siCopy = new EmailSearchData()
            {
                Name = si.Name,
                ID = Guid.NewGuid(),
                Path = si.Path,
                Type = TypeSearchItem.Attachment
            };
            siCopy.Path = string.Format("{0}/at=:{1}",siCopy.Path,row.Cells[3].Value.ToString());

            EventHandler<Services.EventArgs<BaseSearchData>> temp = SelectedItemChanged;
            if (temp != null)
            {
                temp(this, new Services.EventArgs<BaseSearchData>(siCopy));
            }

        }

        private void checkBoxAttachments_CheckedChanged(object sender, EventArgs e)
        {
            dataGridViewEmail.Columns[3].Visible = dataGridViewEmail.Columns[4].Visible = checkBoxAttachments.Checked;
        }



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
