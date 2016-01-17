using System.IO;
using System.Linq;
using Microsoft.Office.Interop.Outlook;
using OF.Core.Data;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Module.Interface.ViewModel;
using OFPreview.PreviewHandler.Service.OutlookPreview;

namespace OF.Module.Core
{
    public class OFBaseEmailPreviewCommand : OFBasePreviewCommand
    {
        public OFBaseEmailPreviewCommand(IMainViewModel mainViewModel) : base(mainViewModel)
        {
        }

        protected override bool OnCanExecute()
        {
            if (MainViewModel != null &&
                (
                (MainViewModel.IsPreviewVisible && MainViewModel.Current != null && (MainViewModel.Current.TypeItem & OFTypeSearchItem.Email) == OFTypeSearchItem.Email)
                ||
                (!MainViewModel.IsPreviewVisible && MainViewModel.CurrentTracked != null && (MainViewModel.CurrentTracked.TypeItem & OFTypeSearchItem.Email) == OFTypeSearchItem.Email)
                )
                )
                return true;
            return false;
        }


        protected void InsertAttachments(MailItem email, OFEmailSearchObject searchEmail)
        {
            var attachments = OutlookPreviewHelper.Instance.GetAttachments(searchEmail);
            attachments.Where(s => !string.IsNullOrEmpty(s)).ForEach(s =>
            {
                email.Attachments.Add(s, OlAttachmentType.olByValue, 1, Path.GetFileName(s));
            });
        }


    }
}