using System;
using System.Diagnostics;
using System.Windows;
using WSUI.Core.Enums;
using WSUI.Core.Helpers;
using WSUI.Core.Logger;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using WSUI.Module.Interface.ViewModel;
using WSUI.Module.Service.Dialogs.Message;

namespace WSUI.Module.Commands
{
    public class OpenEmailCommand :  BasePreviewCommand
    {
        public OpenEmailCommand(IMainViewModel mainViewModel) : base(mainViewModel)
        {
        }

        protected override bool OnCanExecute()
        {
            if (MainViewModel != null && MainViewModel.Current != null &&
                (MainViewModel.Current.TypeItem & TypeSearchItem.Email) == MainViewModel.Current.TypeItem)
                return true;
            return false;
        }
        protected override void OnExecute()
        {
            string filename = string.Empty;
            try
            {
                switch (MainViewModel.Current.TypeItem)
                {
                    case TypeSearchItem.Email:
                        filename = TempFileManager.Instance.GenerateTempFileName(MainViewModel.Current);
                        break;
                }
                if(string.IsNullOrEmpty(filename))
                    return;
                Process.Start(filename);

            }
            catch (Exception ex)
            {
                MessageBoxService.Instance.Show("Error",
                                                string.Format(
                                                    "Error: '{0}'",ex.Message) , MessageBoxButton.OK, MessageBoxImage.Error);
                WSSqlLogger.Instance.LogError(ex.Message);
            }
        }

        protected override string GetIcon()
        {
            return @"pack://application:,,,/WSUI.Module;component/Images/openemail.png";
        }

        protected override string GetCaption()
        {
            return "Open";
        }

        protected override string GetTooltip()
        {
            return "Open Email";
        }
    }
}