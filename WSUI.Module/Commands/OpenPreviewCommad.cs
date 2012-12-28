using System.Diagnostics;
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

            if (KindItem != null && KindItem.Current != null &&
                ((KindItem.Current.Type & TypeSearchItem.FileAll) == KindItem.Current.Type))
                return true;
            return false;
        }

        protected override void OnExecute()
        {
            string fileName = string.Empty;
            switch (KindItem.Current.Type)
            {
                case TypeSearchItem.File:
                case TypeSearchItem.Picture:
                case TypeSearchItem.FileAll:
                    fileName = SearchItemHelper.GetFileName(KindItem.Current,false);
                    break;
                case TypeSearchItem.Attachment:
                    fileName = TempFileManager.Instance.GenerateTempFileName(KindItem.Current) ?? OutlookHelper.Instance.GetAttachmentTempFileName(KindItem.Current);
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
