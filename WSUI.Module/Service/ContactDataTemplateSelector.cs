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
    public class ContactDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate EmailTemplate { get; set; }
        public DataTemplate ContactTemplate { get; set; }
        
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var bs = item as BaseSearchData;
            switch (bs.Type)
            {
                case TypeSearchItem.Email:
                    return EmailTemplate;
                case TypeSearchItem.Contact:
                    return ContactTemplate;
            }
            return null;
        }

    }
}
