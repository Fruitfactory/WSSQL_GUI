﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using OFPreview.PreviewHandler.PreviewHandlerHost;
using OF.Core.Data;
using OF.Core.Enums;
using OF.Core.EventArguments;
using OF.Core.Interfaces;
using OF.Module.Interface;
using OF.Module.Interface.View;
using OF.Module.ViewModel;
using OF.Core.Win32;

namespace OF.Module.View
{
    /// <summary>
    /// Interaction logic for PreviewView.xaml
    /// </summary>
    public partial class PreviewView : IPreviewView, INavigationView
    {
        public PreviewView()
        {
            InitializeComponent();
            _previewControl.StartLoad += PreviewControlOnStartLoad;
            _previewControl.StopLoad += PreviewControlOnStopLoad;
            _previewControl.CommandExecuted += PreviewControlOnCommandExecuted;

        }

        private void PreviewControlOnCommandExecuted(object sender, OFPreviewCommandArgs wsuiPreviewCommandArgs)
        {
            switch (wsuiPreviewCommandArgs.PreviewCommand)
            {
                case OFPreviewCommand.ShowFolder:
                    Model.ShowOutlookFolder(wsuiPreviewCommandArgs.Tag as string);
                    break;
                case OFPreviewCommand.ShowContact:
                    Model.ShowContactPreview(wsuiPreviewCommandArgs.Tag);
                    break;
            }
        }

        private void PreviewControlOnStopLoad(object sender, EventArgs eventArgs)
        {
            var temp = StopLoad;
            if (temp != null)
            {
                temp(this, EventArgs.Empty);
            }
        }

        private void PreviewControlOnStartLoad(object sender, EventArgs eventArgs)
        {
            var temp = StartLoad;
            if (temp != null)
            {
                temp(this, EventArgs.Empty);
            }
        }

        #region Implementation of IPreviewView

        public MainViewModel Model
        {
            get { return DataContext as MainViewModel; }
            set { DataContext = value; }
        }
        public bool SetPreviewFile(string filename)
        {
            if (_previewControl == null)
                return false;
            _previewControl.FilePath = filename;
            return true;
        }

        public bool SetPreviewObject(OFBaseSearchObject searchObject)
        {
            if (_previewControl == null)
                return false;
            _previewControl.PreviewForSearchObject(searchObject);
            return true;
        }

        public void SetSearchPattern(string pattern)
        {
            if (!string.IsNullOrEmpty(pattern))
                _previewControl.SearchCriteria = pattern;
        }

        public void SetFullFolderPath(string path)
        {
            _previewControl.FullFolderPath = path;
        }

        public void ClearPreview()
        {
            _previewControl.UnloadPreview();
        }

        public void PassActionForPreview(IWSAction action)
        {
            _previewControl.PassAction(action);
        }

        public event EventHandler StartLoad;
        public event EventHandler StopLoad;

        #endregion


        public void Init()
        {

        }


    }
}
