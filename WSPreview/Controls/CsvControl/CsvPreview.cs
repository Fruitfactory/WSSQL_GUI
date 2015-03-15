using System.Data;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using WSPreview.PreviewHandler.PreviewHandlerFramework;
using WSPreview.PreviewHandler.Service.CsvHelper;
using WSUI.Core.Data;

namespace WSPreview.PreviewHandler.Controls.CsvControl
{
    [KeyControl(ControlsKey.Csv)]
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

        public void LoadObject(BaseSearchObject obj)
        {
            
        }

        public void Clear()
        {
            if (dataGridCsv != null)
            {
                dataGridCsv.DataSource = null;
            }
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
