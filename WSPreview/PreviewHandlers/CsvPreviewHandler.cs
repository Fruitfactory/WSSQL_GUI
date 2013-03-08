// Stephen Toub
// Coded and published in January 2007 issue of MSDN Magazine 
// http://msdn.microsoft.com/msdnmag/issues/07/01/PreviewHandlers/default.aspx

using System;
using System.IO;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Globalization;
using C4F.DevKit.PreviewHandler.PreviewHandlerFramework;

namespace C4F.DevKit.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("MSDN Magazine CSV Preview Handler", "", "{5F1DA711-99CA-4C7B-B314-90DD9D23E525}")]
    [ProgId("C4F.DevKit.PreviewHandler.PreviewHandlers.CsvPreviewHandler")]
    [Guid("9834EBE8-DA5E-465E-9C51-3B5E4F13C015")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public sealed class CsvPreviewHandler : StreamBasedPreviewHandler
    {
        

        protected override PreviewHandlerControl CreatePreviewHandlerControl()
        {
            return new CsvPreviewHandlerControl();
        }

        private sealed class CsvPreviewHandlerControl : StreamBasedPreviewHandlerControl
        {
            private readonly static char[] _mostUsefulSeparator = new char[] { ',', ';', '\t', ':' ,'|'};

            public override void Load(Stream stream)
            {
                DataGridView grid = new DataGridView();
                grid.DataSource = ParseCsv(stream);
                grid.ReadOnly = true;
                grid.Dock = DockStyle.Fill;
                grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                Controls.Add(grid);
            }

            private static DataTable ParseCsv(Stream stream)
            {
                DataTable table = new DataTable();
                table.Locale = CultureInfo.CurrentCulture;
                table.TableName = stream is FileStream ? ((FileStream)stream).Name : "CSV";

                List<string[]> lines = new List<string[]>();
                int maxFields = 0;

                using (StreamReader reader = new StreamReader(stream))
                {
                    string line = reader.ReadLine();
                    if(line == null)
                        return table;
                    char sep = GetSeparator(line);
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (line.Length > 0)
                        {
                            string[] parts = line.Split(sep);
                            maxFields = Math.Max(maxFields, parts.Length);
                            lines.Add(parts);
                        }
                    }
                }

                if (lines.Count > 0 && maxFields > 0)
                {
                    for (int i = 0; i < maxFields; i++) table.Columns.Add("Column " + i);
                    foreach (object[] line in lines)
                    {
                        table.Rows.Add(line);
                    }
                }

                return table;
            }

            private static char GetSeparator(string line)
            {
                int index = -1;
                char ch = ',';
                foreach (var sep in _mostUsefulSeparator)
                {
                    if((index = line.IndexOf(sep)) > -1)
                    {
                        ch = sep;
                        break;
                    }
                }
                return ch;
            }
        }
    }
}