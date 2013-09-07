using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using WSUI.Core.Enums;
using WSUI.Core.Helpers;
using WSUI.Core.Logger;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using WSUI.Module.Service.Dialogs.Message;

namespace WSUI.Module.Commands
{
    public class OpenEmailCommand :  BasePreviewCommand
    {
        public OpenEmailCommand(IKindItem kindItem) : base(kindItem)
        {
        }

        protected override bool OnCanExecute()
        {
            if (KindItem != null && KindItem.Current != null &&
                (KindItem.Current.Type & TypeSearchItem.Email) == KindItem.Current.Type)
                return true;
            return false;
        }
        protected override void OnExecute()
        {
            string filename = string.Empty;
            try
            {
                switch (KindItem.Current.Type)
                {
                    case TypeSearchItem.Email:
                        filename = TempFileManager.Instance.GenerateTempFileName(KindItem.Current);
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