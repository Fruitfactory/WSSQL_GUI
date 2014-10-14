using System;
using System.Diagnostics;
using System.Windows;
using WSUI.Core.Enums;
using WSUI.Core.Helpers;
using WSUI.Core.Logger;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using WSUI.Module.Interface.ViewModel;
using WSUI.Module.Service.Dialogs.Message;

namespace WSUI.Module.Commands
{
    public class OpenEmailCommand :  BaseEmailPreviewCommand
    {
        public OpenEmailCommand(IMainViewModel mainViewModel) : base(mainViewModel)
        {
        }
        protected override void OnExecute()
        {
            var type = GetTypeOfCurrentItem();
            var currentItem = GetCurrentSearchObject();

            string filename = string.Empty;
            try
            {
                switch (type)
                {
                    case TypeSearchItem.Email:
                        filename = MainViewModel.IsPreviewVisible ? 
                            TempFileManager.Instance.GenerateTempFileName(currentItem) :
                            SearchItemHelper.GetFileName(currentItem);
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