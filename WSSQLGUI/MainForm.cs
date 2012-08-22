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

namespace WSSQLGUI
{
    
    public partial class MainForm : Form
    {
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
            }
        }


        private void AddRowsToGrid(string Value)
        {
            dataGridView1.Rows.Add(Value);
        }

        private delegate void querydelegate(string value);

        private void SearchButton_Click(object sender, EventArgs e)
        {
            
            Thread tThread = new Thread(() => query(SearchTextBox.Text));
            tThread.Start();
            
        }

       
    }
}
