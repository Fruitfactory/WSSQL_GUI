using System;
using System.Diagnostics;
using System.Windows;
using OF.Core.Enums;
using OF.Core.Helpers;
using OF.Core.Logger;
using OF.Infrastructure.Service.Helpers;
using OF.Module.Core;
using OF.Module.Interface;
using OF.Module.Interface.ViewModel;
using OF.Module.Service.Dialogs.Message;

namespace OF.Module.Commands
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
                        filename = SearchItemHelper.GetFileName(currentItem);
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
                OFLogger.Instance.LogError(ex.Message);
            }
        }

        protected override string GetIcon()
        {
            return @"pack://application:,,,/OF.Module;component/Images/openemail.png";
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