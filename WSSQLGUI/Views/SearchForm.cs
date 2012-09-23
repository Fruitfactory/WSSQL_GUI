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
using System.Text.RegularExpressions;



namespace WSSQLGUI.Views
{
    [View(typeof(MainTask), MainTask.Search)]
    public partial class SearchForm : WinFormView
    {

        private Font _defaultFont;
        private bool _isLoading = false;


        public SearchForm()
        {
            InitializeComponent();
            _defaultFont = dataGridView1.DefaultCellStyle.Font;
        }

        public override void Initialize()
        {
            base.Initialize();
            if (Controller != null)
            {
                (Controller as SearchController).OnStartSearch += (sender,e) => this.Invoke(new Action<object,EventArgs>(StartSearch),new object[]{sender,e});
                (Controller as SearchController).OnCompleteSearch += (sender, e) => this.Invoke(new Action<object, EventArgs<bool>>(CompleteSearch), new object[] { sender, e });
                (Controller as SearchController).OnAddSearchItem += AddSearchItem;
                commandManager.Bind((Controller as SearchController).OpenFileCommand, buttonPreview);
                commandManager.Bind((Controller as SearchController).OpenFileCommand, toolStripMenuItemOpen);
                commandManager.Bind((Controller as SearchController).SearchCommand, SearchButton);
            }

        }

        private void StartSearch(object sender, EventArgs e)
        {
            SearchTextBox.Enabled = SearchButton.Enabled = false;
            this.Cursor = dataGridView1.Cursor = previewControl.Cursor = Cursors.AppStarting;
            dataGridView1.Rows.Clear();
            previewControl.UnloadPreview();
        }

        private void CompleteSearch(object sender, EventArgs<bool> e)
        {
            _isLoading = false;
            SearchTextBox.Enabled = SearchButton.Enabled = true;
            this.Cursor = dataGridView1.Cursor = previewControl.Cursor = Cursors.Default;
            dataGridView1.ClearSelection();
        }

        private void AddSearchItem(object sender, EventArgs<SearchItem> e)
        {
            this.Invoke(new Action(() => 
                {
                    int i = dataGridView1.Rows.Add(new object[] { e.Value.Recepient, e.Value.Subject,e.Value.Date });
                    dataGridView1.Rows[i].Tag = e.Value;
                }),null);
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (_isLoading)
                return;
            if (dataGridView1.SelectedCells.Count == 0)
                return;

            SearchItem si = dataGridView1.SelectedRows[0].Tag as SearchItem;
            
            (Controller as SearchController).CurrentSearchItemChanged(si);

            string filename = (Controller as SearchController).FileName;

            if (FileService.IsDirectory(filename) || !File.Exists(filename))
            {
                previewControl.FilePath = string.Empty;
                return;
            }

            previewControl.FilePath = filename;
        }

        private void SearchTextBox_Validated(object sender, EventArgs e)
        {
            string pattern = @"\bselect\s.*from\b";
            Regex reg = new Regex(pattern,RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = reg.Match(SearchTextBox.Text);
            if (m.Success)
            {
                errorProvider.SetError(SearchTextBox, "You have written wrong Search Criteria");
                (Controller as SearchController).SearchCriteria = string.Empty;
            }
            else
            {
                errorProvider.SetError(SearchTextBox, "");
                (Controller as SearchController).SearchCriteria = SearchTextBox.Text;
            }
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            SearchTextBox_Validated(sender, e);
            
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.Rows[e.RowIndex].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        }

        private void dataGridView1_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.Rows[e.RowIndex].DefaultCellStyle.Font = _defaultFont;
        }
               
    }
}
