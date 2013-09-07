using System.Windows;
using System.Windows.Controls;
using WSUI.Core.Core;
using WSUI.Core.Enums;

namespace WSUI.Module.Service
{
    public class ContactDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NoneTemplate { get; set; }
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
                default:
                    return NoneTemplate;
            }
            return null;
        }

    }
}
