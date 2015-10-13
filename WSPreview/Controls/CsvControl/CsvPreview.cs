using System.Data;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using OFPreview.PreviewHandler.PreviewHandlerFramework;
using OFPreview.PreviewHandler.Service.CsvHelper;
using OF.Core.Data;

namespace OFPreview.PreviewHandler.Controls.CsvControl
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

        public void LoadObject(OFBaseSearchObject obj)
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
