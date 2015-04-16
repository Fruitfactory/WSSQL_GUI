// Stephen Toub
// Coded and published in January 2007 issue of MSDN Magazine 
// http://msdn.microsoft.com/msdnmag/issues/07/01/PreviewHandlers/default.aspx

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Security.Permissions;
using System.ComponentModel;
using OFPreview.PreviewHandler.PreviewHandlerFramework;

namespace OFPreview.PreviewHandler.PreviewHandlers.PreviewHandlers
{
    [PreviewHandler("MSDN Magazine MSI Preview Handler", ".msi", "{5CAD7435-A7CD-495C-A84F-A697E4D81C06}")]
    [ProgId("OFPreview.PreviewHandler.PreviewHandlers.MsiPreviewHandler")]
    [Guid("F90F8E43-EF26-4321-A172-5502E8E63536")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public sealed class MsiPreviewHandler : FileBasedPreviewHandler
    {
        protected override PreviewHandlerControl CreatePreviewHandlerControl()
        {
            return new MsiPreviewHandlerControl();
        }

        private sealed class MsiPreviewHandlerControl : FileBasedPreviewHandlerControl
        {
            public override void Load(FileInfo file)
            {
                ListView listView = new ListView();

                listView.Columns.Add("File Name", -2);
                listView.Columns.Add("Size", -2);
                listView.Columns.Add("Version", -2);
                
                listView.Dock = DockStyle.Fill;
                listView.BorderStyle = BorderStyle.None;

                listView.FullRowSelect = true;
                listView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
                listView.MultiSelect = false;
                listView.LargeImageList = listView.SmallImageList = _iconProvider.Icons;
                listView.View = View.Details;

                foreach (MsiFileInfo msiFile in GetFiles(file.FullName))
                {
                    listView.Items.Add(new ListViewItem(
                        new string[] { msiFile.FileName, msiFile.Size, msiFile.Version },
                        _iconProvider.GetIconIndexForFile(msiFile.FileName)));
                }
                Controls.Add(listView);
            }

            private static List<MsiFileInfo> GetFiles(string msiPath)
            {
                List<MsiFileInfo> msiFiles = new List<MsiFileInfo>();

                // Open database
                int dbHandleVal;
                if (MsiOpenDatabase(msiPath, IntPtr.Zero, out dbHandleVal) != 0) throw new Win32Exception();
                using (MsiHandle dbHandle = new MsiHandle(dbHandleVal))
                {
                    // Make sure file table exists
                    string tableName = "File";
                    if (MsiDatabaseIsTablePersistent(dbHandle.Handle, tableName) == 1)
                    {
                        // Opoen the view of the list of files
                        string query = "SELECT * FROM `" + tableName + "`";
                        int viewHandleVal;
                        if (MsiDatabaseOpenView(dbHandle.Handle, query, out viewHandleVal) != 0) throw new Win32Exception();
                        using (MsiHandle viewHandle = new MsiHandle(viewHandleVal))
                        {
                            if (MsiViewExecute(viewHandle.Handle, 0) != 0) throw new Win32Exception();

                            // Get a list of the columns in order to find the name field
                            Dictionary<string, int> fieldToIndexMapping = new Dictionary<string, int>();
                            int columnNamesRecordVal;
                            if (MsiViewGetColumnInfo(viewHandle.Handle, 0, out columnNamesRecordVal) != 0) throw new Win32Exception();
                            using (MsiHandle columnNamesRecord = new MsiHandle(columnNamesRecordVal))
                            {
                                int fieldCount = MsiRecordGetFieldCount(columnNamesRecord.Handle);
                                for (int i = 0; i < fieldCount; i++)
                                {
                                    fieldToIndexMapping[GetRecordString(columnNamesRecord, i)] = i;
                                }
                            }
                            if (fieldToIndexMapping.ContainsKey("FileName"))
                            {
                                // Get the files list
                                uint rv;
                                int recordVal;
                                while ((rv = MsiViewFetch(viewHandle.Handle, out recordVal)) == 0)
                                {
                                    using (MsiHandle record = new MsiHandle(recordVal))
                                    {
                                        string[] fileNameParts = GetRecordString(record, fieldToIndexMapping["FileName"]).Split(
                                            new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                                        string fileName = fileNameParts.Length > 1 ? fileNameParts[1] : fileNameParts[0];
                                        string size = GetRecordString(record, fieldToIndexMapping["FileSize"]);
                                        string version = GetRecordString(record, fieldToIndexMapping["Version"]);
                                        msiFiles.Add(new MsiFileInfo(fileName, version, size));
                                    }
                                }
                                if (rv != 0x103) throw new Win32Exception();
                            }
                        }
                    }
                }
                return msiFiles;
            }

            private struct MsiFileInfo
            {
                public MsiFileInfo(string fileName, string version, string size) { FileName = fileName; Version = version; Size = size; }
                public readonly string FileName;
                public readonly string Version;
                public readonly string Size;
            }

            private static string GetRecordString(MsiHandle record, int field)
            {
                int length = 255;
                uint rv = 0;
                StringBuilder text = new StringBuilder(length);
                if ((rv = MsiRecordGetString(record.Handle, field, text, ref length)) == 0xea)
                {
                    text.EnsureCapacity(++length);
                    rv = MsiRecordGetString(record.Handle, field, text, ref length);
                }
                if (rv != 0) throw new Win32Exception();
                return text.ToString();
            }

            [DllImport("msi.dll", EntryPoint = "MsiOpenDatabaseW", CharSet = CharSet.Unicode, ExactSpelling = true)]
            private static extern uint MsiOpenDatabase(string databasePath, IntPtr persist, out int database);

            [DllImport("msi.dll", EntryPoint = "MsiDatabaseIsTablePersistentW", CharSet = CharSet.Unicode, ExactSpelling = true)]
            private static extern int MsiDatabaseIsTablePersistent(int database, string tableName);

            [DllImport("msi.dll", EntryPoint = "MsiDatabaseOpenViewW", CharSet = CharSet.Unicode, ExactSpelling = true)]
            private static extern uint MsiDatabaseOpenView(int database, string query, out int view);

            [DllImport("msi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
            private static extern uint MsiViewExecute(int view, int record);

            [DllImport("msi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
            private static extern uint MsiViewFetch(int view, out int record);

            [DllImport("msi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
            private static extern uint MsiViewGetColumnInfo(int view, int columnInfo, out int record);

            [DllImport("msi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
            private static extern int MsiRecordGetFieldCount(int record);

            [DllImport("msi.dll", EntryPoint = "MsiRecordGetStringW", CharSet = CharSet.Unicode, ExactSpelling = true)]
            private static extern uint MsiRecordGetString(int record, int field, StringBuilder valueBuf, ref int valueBufSize);

            // An MSI handle is a 32-bit value, even on 64-bit platforms.  As such,
            // an actual SafeHandle-derived type is inappropriate.
            private sealed class MsiHandle : CriticalFinalizerObject, IDisposable
            {
                private int _handle;

                public MsiHandle(int handle) { _handle = handle; }

                public int Handle { get { return _handle; } }

                [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
                [DllImport("msi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
                private static extern uint MsiCloseHandle(int database);

                void IDisposable.Dispose()
                {
                    GC.SuppressFinalize(this);
                    Dispose();
                }

                [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
                private void Dispose()
                {
                    if (_handle != 0) MsiCloseHandle(_handle);
                    _handle = 0;
                }

                ~MsiHandle() { Dispose(); }
            }

            private FileTypeIconProvider _iconProvider = new FileTypeIconProvider();

            protected override void Dispose(bool disposing)
            {
                if (disposing && _iconProvider != null)
                {
                    _iconProvider.Dispose();
                    _iconProvider = null;
                }
                base.Dispose(disposing);
            }
        }
    }
}