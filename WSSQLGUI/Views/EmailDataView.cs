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
using WSSQLGUI.Models;
using WSSQLGUI.Core;
using MVCSharp.Core.Configuration.Views;
using MVCSharp.Core;

namespace WSSQLGUI.Views
{
    [View(typeof(EmailDataTask), EmailDataTask.EmailDataView)]
    internal partial class EmailDataView : WinUserControlView, IEmailDataView
    {
        private EmailSearchData _current;

        public EmailDataView()
        {
            InitializeComponent();
            dataGridViewEmail.SelectionChanged += DataGridSelectionChanged;
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
                    dataGridViewEmail.Rows[i].Tag = data;
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

    }
}
