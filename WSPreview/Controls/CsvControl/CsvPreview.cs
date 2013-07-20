﻿using System.Data;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using WSPreview.PreviewHandler.PreviewHandlerFramework;
using WSPreview.PreviewHandler.Service.CsvHelper;

namespace WSPreview.PreviewHandler.Controls.CsvControl
{
    public partial class CsvPreview : UserControl, IPreviewControl
    {
        public CsvPreview()
        {
            InitializeComponent();
        }

        public void LoadFile(string filename)
        {
        }

        public void LoadFile(Stream stream)
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = ParseCsv(stream);
            dataGridCsv.DataSource = bs;
            dataGridCsv.ReadOnly = true;
            dataGridCsv.Dock = DockStyle.Fill;
            dataGridCsv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private static DataTable ParseCsv(Stream stream)
        {
            DataTable table = new DataTable();

            using (StreamReader reader = new StreamReader(stream))
            {
                table = CSV.LoadDataTable(reader, true, true);
                table.Locale = CultureInfo.CurrentCulture;
                table.TableName = stream is FileStream ? ((FileStream)stream).Name : "CSV";
                reader.Close();
            }
            return table;
        }

    }
}
