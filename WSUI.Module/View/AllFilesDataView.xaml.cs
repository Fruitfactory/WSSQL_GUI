using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using WSUI.Infrastructure.Service;
using WSUI.Module.Interface;
using WSUI.Module.Interface.View;
using WSUI.Module.ViewModel;
using WSUI.Core.Interfaces;

namespace WSUI.Module.View
{
    /// <summary>
    /// Interaction logic for AllFilesDataView.xaml
    /// </summary>
    public partial class AllFilesDataView : IDataView<AllFilesViewModel>,INavigationView
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

    }
}
