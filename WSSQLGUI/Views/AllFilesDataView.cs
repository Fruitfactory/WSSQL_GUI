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
using WSSQLGUI.Controls.PagingControl;

namespace WSSQLGUI.Views
{
    [View(typeof(AllFilesDataTask), AllFilesDataTask.AllFilesDataView)]
    internal partial class AllFilesDataView : WinUserControlView,IAllFilesDataView
    {

        private BaseSearchData _current;
        private PagingDataSource<BaseSearchData> _pagingDataView;  


        public AllFilesDataView()
        {
            InitializeComponent();
#region init paging

            _pagingDataView = new PagingDataSource<BaseSearchData>();

            var colName = new DataGridViewTextBoxColumn();
            var colPath = new DataGridViewTextBoxColumn();
            colName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colName.HeaderText = "Name";
            colName.DataPropertyName = "Name";
            colName.MinimumWidth = 150;
            colName.Name = "columnName";
            colName.ReadOnly = true;
            colName.Width = 150;
            
            colPath.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colPath.HeaderText = "Path";
            colPath.DataPropertyName = "Path";
            colPath.MinimumWidth = 200;
            colPath.Name = "columnPath";
            colPath.ReadOnly = true;
            colPath.Width = 200;

            _pagingDataView.AddColumn(colName);
            _pagingDataView.AddColumn(colPath);

            this.Controls.Add(_pagingDataView);
            _pagingDataView.Dock = DockStyle.Fill;
            _pagingDataView.SelectedChanged += DataGridSelectionChanged;

            #endregion
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
            _pagingDataView.AddData(addData);
        }

        public event EventHandler<Services.EventArgs<BaseSearchData>> SelectedItemChanged;


        public void SetContextMenu(ContextMenuStrip contextMenu)
        {
            _pagingDataView.ContextMenuStrip = contextMenu;
        }

        public bool IsLoading
        {
            get;
            set;
        }

        public void Clear()
        {
            _pagingDataView.ClearRows();
        }

        private void DataGridSelectionChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;
            
            if (_pagingDataView.Selected == null)
                return;

            BaseSearchData si = _pagingDataView.Selected;
            _current = si;
            EventHandler<Services.EventArgs<BaseSearchData>> temp = SelectedItemChanged;
            if (temp != null)
            {
                temp(this, new Services.EventArgs<BaseSearchData>(si));
            }
        }



        public void StartLoading()
        {
            _pagingDataView.StartLoading();
        }

        public void FinishLoading()
        {
            Invoke(new Action(() => _pagingDataView.CompleteLoading()), null);
        }

        public void SetData(IList<BaseSearchData> listData)
        {
            
        }
    }
}
