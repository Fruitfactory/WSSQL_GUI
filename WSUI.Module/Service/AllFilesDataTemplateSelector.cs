using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using WSUI.Infrastructure.Core;
using WSUI.Infrastructure.Service.Enums;

namespace WSUI.Module.Service
{
    public class AllFilesDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NoneTemplate { get; set; }
        public DataTemplate EmailTemplate { get; set; }
        public DataTemplate UserTemplate { get; set; }
        public DataTemplate FileTemplate { get; set; }
        public DataTemplate AttachmentTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var bs = item as BaseSearchData;
            switch (bs.Type)
            {
                case TypeSearchItem.None:
                    return NoneTemplate;
                case TypeSearchItem.Email:
                    return EmailTemplate;
                case TypeSearchItem.Contact:
                    return UserTemplate;
                case TypeSearchItem.File:
                    return FileTemplate;
                case TypeSearchItem.Attachment:
                    return AttachmentTemplate;
                default:
                    return NoneTemplate;
            }
        }

    }
}
