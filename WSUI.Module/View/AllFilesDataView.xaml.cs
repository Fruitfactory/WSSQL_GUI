﻿using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using OF.Infrastructure.Service;
using OF.Module.Interface;
using OF.Module.Interface.View;
using OF.Module.ViewModel;
using OF.Core.Interfaces;

namespace OF.Module.View
{
    /// <summary>
    /// Interaction logic for AllFilesDataView.xaml
    /// </summary>
    public partial class AllFilesDataView : IDataView<AllFilesViewModel>,INavigationView, IAllDataView
    {
        public AllFilesDataView()
        {
            InitializeComponent();
            ContactListBox.SelectionChanged += ContactListBoxOnSelectionChanged;
            listBox.SelectionChanged += ListBoxOnSelectionChanged;
            FileListBox.SelectionChanged += FileListBoxOnSelectionChanged;
        }

        private void FileListBoxOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            listBox.SelectedIndex = -1;
            ContactListBox.SelectedIndex = -1;
        }

        private void ListBoxOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            ContactListBox.SelectedIndex = -1;
            FileListBox.SelectedIndex = -1;
        }

        private void ContactListBoxOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            listBox.SelectedIndex = -1;
            FileListBox.SelectedIndex = -1;
        }

        #region Implementation of IDataView<AllFilesViewModel>

        public AllFilesViewModel Model 
        {
            get { return DataContext as AllFilesViewModel; }
            set { DataContext = value; }
        }

        #endregion

        public double ActualContactHeight { get { return ContactControl.ActualHeight; } }
        public double ActualFileHeight 
        {
            get
            {
                return FileControl.ActualHeight;
            } 
        }
        public double ActualGridHeight { get { return AllDataRootGrid.ActualHeight; } }
    }
}
