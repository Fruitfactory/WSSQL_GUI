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
using WSSQLGUI.Controllers.Tasks;
using WSSQLGUI.Models;
using WSSQLGUI.Services;
using System.Text.RegularExpressions;
using WSSQLGUI.Core;



namespace WSSQLGUI.Views
{
    [View(typeof(MainTask), MainTask.Search)]
    public partial class SearchForm : WinFormView
    {
        public SearchForm()
        {
            InitializeComponent();
        }

        public override void Initialize()
        {
            base.Initialize();
            if (Controller != null)
            {
                (Controller as SearchController).OnStartSearch += (sender,e) => this.Invoke(new Action<object,EventArgs>(StartSearch),new object[]{sender,e});
                (Controller as SearchController).OnCompleteSearch += (sender, e) => this.Invoke(new Action<object, EventArgs<bool>>(CompleteSearch), new object[] { sender, e });
                (Controller as SearchController).OnItemChanged += ItemChanged;
                commandManager.Bind((Controller as SearchController).OpenFileCommand, buttonPreview);
                commandManager.Bind((Controller as SearchController).OpenFileCommand, toolStripMenuItemOpen);
                var list = (Controller as SearchController).GetAllKinds();
                comboBoxKinds.DataSource = list;
            }

        }


        public Panel SettingsPanel
        {
            get { return panelSettings; }
        }

        public Panel DataPanel
        {
            get
            {
                return splitPanels.Panel1;
            }
        }

        private void StartSearch(object sender, EventArgs e)
        {
            this.Cursor =  previewControl.Cursor = Cursors.AppStarting;
            previewControl.UnloadPreview();
        }

        private void CompleteSearch(object sender, EventArgs<bool> e)
        {
            this.Cursor = previewControl.Cursor = Cursors.Default;
        }

        private void ItemChanged(object sender, EventArgs e)
        {
            string filename = (Controller as SearchController).FileName;

            if (FileService.IsDirectory(filename) || !File.Exists(filename))
            {
                previewControl.FilePath = string.Empty;
                return;
            }

            previewControl.FilePath = filename;
        }

        private void comboBoxKinds_SelectedIndexChanged(object sender, EventArgs e)
        {
            var controller = Controller as SearchController;
            if (controller == null)
                return;
            controller.CurrentKindChanged(comboBoxKinds.SelectedValue.ToString());
            previewControl.UnloadPreview();
        }
    }
}
