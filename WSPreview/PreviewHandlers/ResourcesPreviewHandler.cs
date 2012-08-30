// Stephen Toub
// Coded and published in January 2007 issue of MSDN Magazine 
// http://msdn.microsoft.com/msdnmag/issues/07/01/PreviewHandlers/default.aspx

using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Resources;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using C4F.DevKit.PreviewHandler.PreviewHandlerFramework;

namespace C4F.DevKit.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("MSDN Magazine Resources Preview Handler", ".resources", "{0595438C-7C68-4A9B-B5CD-BF6EBAA6623A}")]
    [ProgId("C4F.DevKit.PreviewHandler.PreviewHandlers.ResourcesPreviewHandler")]
    [Guid("7B2D1C25-6DE7-4F16-8C9B-AD65A1EC8FB0")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public sealed class ResourcesPreviewHandler : StreamBasedPreviewHandler
    {
        protected override PreviewHandlerControl CreatePreviewHandlerControl()
        {
            return new ResourcesPreviewHandlerControl();
        }
        private sealed class ResourcesPreviewHandlerControl : StreamBasedPreviewHandlerControl
        {
            public override void Load(Stream previewStream)
            {
                ListView listView = new ListView();

                listView.Columns.Add("File Name", -2);
                listView.Columns.Add("Data Type", -2);

                listView.Dock = DockStyle.Fill;
                listView.BorderStyle = BorderStyle.None;
                listView.FullRowSelect = true;
                listView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
                listView.MultiSelect = false;
                listView.View = View.Details;
                listView.DoubleClick += delegate
                {
                    if (listView.SelectedItems.Count > 0)
                    {
                        ListViewItem clicked = listView.SelectedItems[0];
                        DictionaryEntry entry = (DictionaryEntry)clicked.Tag;

                        if (entry.Value is Bitmap)
                        {
                            Bitmap bmp = (Bitmap)entry.Value;
                            string tempPath = CreateTempPath(".png");
                            bmp.Save(tempPath, ImageFormat.Png);
                            Process.Start(tempPath);
                        }
                        else if (entry.Value is string)
                        {
                            string str = (string)entry.Value;
                            string tempPath = CreateTempPath(".txt");
                            File.WriteAllText(tempPath, str);
                            Process.Start(tempPath);
                        }
                        else if (entry.Value is Stream || entry.Value is byte[])
                        {
                            byte[] buffer = null;
                            if (entry.Value is byte[]) buffer = (byte[])entry.Value;
                            else
                            {
                                try
                                {
                                    Stream valueStream = (Stream)entry.Value;
                                    valueStream.Position = 0;
                                    buffer = new byte[valueStream.Length];
                                    valueStream.Read(buffer, 0, buffer.Length);
                                }
                                catch (IOException) { }
                                catch (NotSupportedException) { }
                            }
                            if (buffer != null)
                            {
                                using (SaveFileDialog sfd = new SaveFileDialog())
                                {
                                    if (sfd.ShowDialog() == DialogResult.OK)
                                    {
                                        File.WriteAllBytes(sfd.FileName, buffer);
                                    }
                                }
                            }
                        }
                    }
                };

                using (ResourceReader reader = new ResourceReader(previewStream))
                {
                    foreach (DictionaryEntry entry in reader)
                    {
                        ListViewItem item = new ListViewItem(new string[] { entry.Key.ToString(), entry.Value.GetType().ToString() });
                        if (!(entry.Value is string || 
                            entry.Value is Bitmap || 
                            entry.Value is Stream ||
                            entry.Value is byte[])) item.ForeColor = SystemColors.GrayText;
                        item.Tag = entry;
                        listView.Items.Add(item);
                    }
                }

                Controls.Add(listView);
            }
        }
    }
}