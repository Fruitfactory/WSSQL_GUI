using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using C4F.DevKit.PreviewHandler.Service.Logger;
using WSUI.Infrastructure.Service.Enums;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Infrastructure.Services;
using WSUI.Module.Core;
using WSUI.Module.Interface;

namespace WSUI.Module.Commands
{
    public class OpenPreviewCommad : BasePreviewCommand
    {
        public OpenPreviewCommad(IKindItem kindItem) : base(kindItem)
        {
        }

        protected override bool OnCanExecute()
        {

            if (_kindItem != null && _kindItem.Current != null &&
                ((_kindItem.Current.Type & TypeSearchItem.FileAll) == _kindItem.Current.Type))
                return true;
            return false;
        }

        protected override void OnExecute()
        {
            string fileName = string.Empty;
            switch (_kindItem.Current.Type)
            {
                case TypeSearchItem.File:
                case TypeSearchItem.Picture:
                case TypeSearchItem.FileAll:
                    fileName = SearchItemHelper.GetFileName(_kindItem.Current,false);
                    break;
                case TypeSearchItem.Attachment:
                    fileName = TempFileManager.Instance.GenerateTempFileName(_kindItem.Current) ?? OutlookHelper.Instance.GetAttachmentTempFileName(_kindItem.Current);
                    break;
            }
            if (string.IsNullOrEmpty(fileName) ||
                FileService.IsDirectory(fileName))
                return;
            try
            {
                Process.Start(fileName);
            }
            catch (System.Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0}: {1} - {2}", "Start error", fileName, ex.Message));
            }
        }

        protected override string GetCaption()
        {
            return "Open";
        }

        protected override string GetIcon()
        {
            return @"pack://application:,,,/WSUI.Module;component/Images/fileopen.png";
        }

        protected override string GetTooltip()
        {
            return "Open file";
        }

    }
}
