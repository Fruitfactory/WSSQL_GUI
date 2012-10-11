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

namespace WSSQLGUI.Views
{
    [View(typeof(AllFilesDataTask), AllFilesDataTask.AllFilesDataView)]
    internal partial class AllFilesDataView : WinUserControlView,IAllFilesDataView
    {

        private BaseSearchData _current;

        public AllFilesDataView()
        {
            InitializeComponent();
            dataGridViewFiles.SelectionChanged += DataGridSelectionChanged;
            
        }

        public override IController Controller
        {
            get
            {
                return base.Controller as AllFilesDataController;
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


        public Core.BaseSearchData CurrentFilelItem
        {
            get
            {
                return _current;
            }
        }

        public void SetData(object[] data,BaseSearchData addData)
        {
            this.Invoke(new Action(() =>
            {
                int i = dataGridViewFiles.Rows.Add(data);
                dataGridViewFiles.Rows[i].Tag = addData;
            }), null);
        }

        public event EventHandler<Services.EventArgs<BaseSearchData>> SelectedItemChanged;


        public void SetContextMenu(ContextMenuStrip contextMenu)
        {
            dataGridViewFiles.ContextMenuStrip = contextMenu;
        }

        public bool IsLoading
        {
            get;
            set;
        }

        public void Clear()
        {
            dataGridViewFiles.Rows.Clear();
        }

        private void DataGridSelectionChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;
            
            if (dataGridViewFiles.SelectedCells.Count == 0)
                return;

            BaseSearchData si = dataGridViewFiles.SelectedRows[0].Tag as BaseSearchData;
            _current = si;
            EventHandler<Services.EventArgs<BaseSearchData>> temp = SelectedItemChanged;
            if (temp != null)
            {
                temp(this, new Services.EventArgs<BaseSearchData>(si));
            }
        }

    }
}
