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

// maybe we should use MVC pattern for this application ?
// I added preview control. It works, when we enter SQL query like "select system.itemurl from systemindex"
// and when sql query is executing, mouse pointer changes to hourglasses


namespace WSSQLGUI
{
    
    public partial class MainForm : Form
    {
        private const string FILEPREFIX = "file:";
        private bool _isLoading = false;


        public MainForm()
        {
            InitializeComponent();
        }

        const string connectionString = "Provider=Search.CollatorDSO;Extended Properties=\"Application=Windows\"";
        private delegate void AddRowsToGridDelegate(string value);    


        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void query(string value)
        {
            this.Invoke(new Action(OnStart), null);
            bool result = true;
            OleDbDataReader myDataReader = null;
            OleDbConnection myOleDbConnection = new OleDbConnection(connectionString);
            OleDbCommand myOleDbCommand = new OleDbCommand(value , myOleDbConnection);
            try
            {
                
                myOleDbConnection.Open();
                myDataReader = myOleDbCommand.ExecuteReader();
                if (!myDataReader.HasRows)
                {
                    MessageBox.Show("There are no rows!");

                }
                else
                {
                    
                    while (myDataReader.Read())
                    {
                        //dataGridView1.Rows.Add(myDataReader.GetValue(0));
                        this.Invoke(new AddRowsToGridDelegate(AddRowsToGrid), myDataReader.GetValue(0));
                    }
                }

                //uint count = 0;
                //DisplayReader(myDataReader, ref count, 0, chapterDepth);
                //Console.WriteLine("Rows+Chapters=" + count);

            }
            catch (System.Data.OleDb.OleDbException oleDbException)
            {

                MessageBox.Show(oleDbException.Message);
                result = false;
            }
            finally
            {
                // Always call Close when done reading.
                if (myDataReader != null)
                {
                    myDataReader.Close();
                    myDataReader.Dispose();
                }
                // Close the connection when done with it.
                if (myOleDbConnection.State == System.Data.ConnectionState.Open)
                {
                    myOleDbConnection.Close();
                }
                this.Invoke(new Action<bool>(OnComplete), result);
            }
        }

        private void OnStart()
        {
            _isLoading = true;
            SearchTextBox.Enabled = SearchButton.Enabled = false;
            this.Cursor = dataGridView1.Cursor = previewControl.Cursor = Cursors.AppStarting;
        }

        private void OnComplete(bool res)
        {
            _isLoading = false;
            SearchTextBox.Enabled = SearchButton.Enabled = true;
            this.Cursor = dataGridView1.Cursor = previewControl.Cursor = Cursors.Default;
        }

        private void AddRowsToGrid(string Value)
        {
            dataGridView1.Rows.Add(Value);
        }

        private delegate void querydelegate(string value);

        private void SearchButton_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            Thread tThread = new Thread(() => query(SearchTextBox.Text));
            tThread.Start();
            
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

            string filename = dataGridView1.SelectedCells[0].Value.ToString();
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
