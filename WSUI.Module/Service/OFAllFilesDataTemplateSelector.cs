using System.Windows;
using System.Windows.Controls;
using OF.Core.Data;
using OF.Core.Enums;

namespace OF.Module.Service
{
    public class OFAllFilesDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NoneTemplate { get; set; }
        public DataTemplate EmailTemplate { get; set; }
        public DataTemplate UserTemplate { get; set; }
        public DataTemplate FileTemplate { get; set; }
        public DataTemplate AttachmentTemplate { get; set; }
        public DataTemplate PictureTemplate { get; set; }
        public DataTemplate CalendarTemplate { get; set; }
        public DataTemplate CommandTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
                return NoneTemplate;

            var bs = item as OFBaseSearchObject;
            switch (bs.TypeItem)
            {
                case OFTypeSearchItem.None:
                    return NoneTemplate;
                case OFTypeSearchItem.Email:
                    return EmailTemplate;
                case OFTypeSearchItem.Contact:
                    return UserTemplate;
                case OFTypeSearchItem.File:
                    return FileTemplate;
                case OFTypeSearchItem.Attachment:
                    return AttachmentTemplate;
                case OFTypeSearchItem.Picture:
                    return PictureTemplate;
                case OFTypeSearchItem.Calendar:
                    return CalendarTemplate;
                case OFTypeSearchItem.Command:
                    return CommandTemplate;
                default:
                    return NoneTemplate;
            }
        }

    }
}
