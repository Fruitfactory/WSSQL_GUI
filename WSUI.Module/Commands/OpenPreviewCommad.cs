using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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
                _kindItem.Current.Type == TypeSearchItem.File)
                return true;
            return false;
        }

        protected override void OnExecute()
        {
            var fileName = SearchItemHelper.GetFileName(_kindItem.Current);
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

    }
}
