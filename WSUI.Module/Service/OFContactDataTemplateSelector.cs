using System.Windows;
using System.Windows.Controls;
using OF.Core.Data;
using OF.Core.Enums;

namespace OF.Module.Service
{
    public class OFContactDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NoneTemplate { get; set; }
        public DataTemplate EmailTemplate { get; set; }
        public DataTemplate ContactTemplate { get; set; }
        
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var bs = item as OFBaseSearchObject;
            switch (bs.TypeItem)
            {
                case OFTypeSearchItem.Email:
                    return EmailTemplate;
                case OFTypeSearchItem.Contact:
                    return ContactTemplate;
                default:
                    return NoneTemplate;
            }
        }

    }
}
