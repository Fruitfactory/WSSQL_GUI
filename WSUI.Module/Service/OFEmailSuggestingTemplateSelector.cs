using System.Windows;
using System.Windows.Controls;
using OF.Core.Data;
using OF.Core.Enums;
using OF.Core.Extensions;

namespace OF.Module.Service
{
    public class OFEmailSuggestingTemplateSelector : DataTemplateSelector
    {

        public DataTemplate ContactNameTemplate { get; set; }

        public DataTemplate ContactEmailTemplate { get; set; }

        public DataTemplate EmailContactTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var contact = item as OFContactSearchObject;
            var emailContact = item as OFEmailContactSearchObject;
            if (contact.IsNotNull())
            {
                if (string.IsNullOrEmpty(contact.FirstName) || string.IsNullOrEmpty(contact.LastName))
                {
                    return ContactEmailTemplate;
                }
                return ContactNameTemplate;
            }
            return emailContact.IsNotNull() ? EmailContactTemplate : null;
        }
    }
}