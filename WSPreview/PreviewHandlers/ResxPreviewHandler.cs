// Stephen Toub
// Coded and published in January 2007 issue of MSDN Magazine 
// http://msdn.microsoft.com/msdnmag/issues/07/01/PreviewHandlers/default.aspx

using System;
using System.IO;
using System.Resources;
using System.Collections;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using OFPreview.PreviewHandler.PreviewHandlerFramework;

namespace OFPreview.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("MSDN Magazine RESX Preview Handler", ".resx", "{860E1DCC-0691-41DB-B879-C79242A6AF24}")]
    [ProgId("OFPreview.PreviewHandler.PreviewHandlers.ResxPreviewHandler")]
    [Guid("27145BEA-45DC-4E09-BDC1-7847AEC8AE6B")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public sealed class ResxPreviewHandler : FileBasedPreviewHandler
    {
        protected override PreviewHandlerControl CreatePreviewHandlerControl()
        {
            return new ResxPreviewHandlerControl();
        }

        private sealed class ResxPreviewHandlerControl : StreamBasedPreviewHandlerControl
        {
            public override void Load(Stream previewStream)
            {
                ListView listView = new ListView();

                listView.Columns.Add("File Name", -2);
                listView.Columns.Add("Data Type", -2);
                listView.Columns.Add("Value", -2);

                listView.Dock = DockStyle.Fill;
                listView.BorderStyle = BorderStyle.None;
                listView.FullRowSelect = true;
                listView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
                listView.MultiSelect = false;
                listView.View = View.Details;

                Environment.CurrentDirectory = Path.GetDirectoryName(((FileStream)previewStream).Name);
                using (ResXResourceReader reader = new ResXResourceReader(previewStream))
                {
                    foreach (DictionaryEntry entry in reader)
                    {
                        ListViewItem item = new ListViewItem(new string[] { entry.Key.ToString(), entry.Value.GetType().ToString(), entry.Value.ToString() });
                        item.Tag = entry;
                        listView.Items.Add(item);
                    }
                }

                Controls.Add(listView);
            }
        }
    }
}