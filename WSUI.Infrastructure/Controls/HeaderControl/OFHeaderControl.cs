using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OF.Infrastructure.Controls.HeaderControl
{
    public class OFHeaderControl : HeaderedContentControl
    {
        static OFHeaderControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OFHeaderControl),new FrameworkPropertyMetadata(typeof(OFHeaderControl)));
        }

        public static readonly DependencyProperty AdditionalHeaderProperty = DependencyProperty.Register(
            "AdditionalHeader", typeof (string), typeof (OFHeaderControl), new PropertyMetadata(default(string)));

        public string AdditionalHeader
        {
            get { return (string) GetValue(AdditionalHeaderProperty); }
            set { SetValue(AdditionalHeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderFontWeightProperty = DependencyProperty.Register(
            "HeaderFontWeight", typeof (FontWeight), typeof (OFHeaderControl), new PropertyMetadata(default(FontWeight)));

        public FontWeight HeaderFontWeight
        {
            get { return (FontWeight) GetValue(HeaderFontWeightProperty); }
            set { SetValue(HeaderFontWeightProperty, value); }
        }

    }
}