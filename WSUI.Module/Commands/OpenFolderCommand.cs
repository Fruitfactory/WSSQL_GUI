using System.Diagnostics;
using System.IO;
using OF.Core.Enums;
using OF.Core.Logger;
using OF.Infrastructure.Service.Helpers;
using OF.Module.Core;
using OF.Module.Interface.ViewModel;

namespace OF.Module.Commands
{
    public class OpenFolderCommand : BaseFilePreviewCommand
    {
        public OpenFolderCommand(IMainViewModel mainViewModel) : base(mainViewModel)
        {
        }

        protected override void OnExecute()
        {
            OpenFolder();
        }

        protected override string GetIcon()
        {
            return @"pack://application:,,,/OF.Module;component/Images/openfolder.png";
        }

        protected override string GetCaption()
        {
            return "Open Folder";
        }

        protected override string GetTooltip()
        {
            return "Open Folder";
        }

        private void OpenItemFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return;
            try
            {
                Process.Start(fileName);
            }
            catch (System.Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0}: {1} - {2}", "OpenItemFile", fileName, ex.Message));
            }
        }

        private void OpenFolder()
        {
            var filename = SearchItemHelper.GetFileName(GetCurrentSearchObject());
            filename = Path.GetDirectoryName(filename);
            OpenItemFile(filename);
        }
    }
}