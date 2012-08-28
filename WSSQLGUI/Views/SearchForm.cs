using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using MVCSharp.Core.Views;
using MVCSharp.Core;
using MVCSharp.Core.Configuration.Views;
using MVCSharp.Winforms;

using WSSQLGUI.Controllers;
using WSSQLGUI.Models;
using WSSQLGUI.Services;

// maybe we should use MVC pattern for this application ?
// I added preview control. It works, when we enter SQL query like "select system.itemurl from systemindex"
// and when sql query is executing, mouse pointer changes to hourglasses


namespace WSSQLGUI.Views
{
    [View(typeof(MainTask), MainTask.Search)]
    public partial class SearchForm : WinFormView
    {
        private const string FILEPREFIX = "file:";
        private bool _isLoading = false;


        public SearchForm()
        {
            InitializeComponent();
        }

        public override void Initialize()
        {
            base.Initialize();
            if (Controller != null)
            {
                (Controller as SearchController).OnStartSearch += (sender,e) => this.Invoke(new Action<object,EventArgs>(StartSearch),new object[]{sender,e});
                (Controller as SearchController).OnCompleteSearch += (sender, e) => this.Invoke(new Action<object, EventArgs<bool>>(CompleteSearch), new object[] { sender, e });
                (Controller as SearchController).OnAddSearchItem += AddSearchItem;
            }

        }

        private void StartSearch(object sender, EventArgs e)
        {
            _isLoading = true;
            SearchTextBox.Enabled = SearchButton.Enabled = false;
            this.Cursor = dataGridView1.Cursor = previewControl.Cursor = Cursors.AppStarting;
        }

        private void CompleteSearch(object sender, EventArgs<bool> e)
        {
            _isLoading = false;
            SearchTextBox.Enabled = SearchButton.Enabled = true;
            this.Cursor = dataGridView1.Cursor = previewControl.Cursor = Cursors.Default;
        }

        private void AddSearchItem(object sener, EventArgs<SearchItem> e)
        {
            this.Invoke(new Action(() => 
                {
                    int i = dataGridView1.Rows.Add(new object[] { e.Value.Name, e.Value.FileName });
                    dataGridView1.Rows[i].Tag = e.Value;
                }),null);
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            (Controller as SearchController).StartSearch(SearchTextBox.Text);
            
        }

        private bool IsDirectory(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return true;
            FileInfo fi = new FileInfo(filename);

            return (fi.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (_isLoading)
                return;
            if (dataGridView1.SelectedCells.Count == 0)
                return;

            SearchItem si = dataGridView1.SelectedRows[0].Tag as SearchItem;
            (Controller as SearchController).CurrentSearchItemChanged(si);

            string filename = si.FileName;
            if (!filename.StartsWith(FILEPREFIX))
            {
                previewControl.FilePath = string.Empty;
                return;
            }

            filename = filename.Substring(FILEPREFIX.Length);

            if (IsDirectory(filename) || !File.Exists(filename))
            {
                previewControl.FilePath = string.Empty;
                return;
            }

            previewControl.FilePath = filename;
        }

       
    }
}
