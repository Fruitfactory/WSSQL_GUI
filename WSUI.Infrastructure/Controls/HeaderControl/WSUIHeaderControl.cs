using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WSUI.Infrastructure.Controls.HeaderControl
{
    public class WSUIHeaderControl : HeaderedContentControl
    {
        static WSUIHeaderControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WSUIHeaderControl),new FrameworkPropertyMetadata(typeof(WSUIHeaderControl)));
        }

        public static readonly DependencyProperty AdditionalHeaderProperty = DependencyProperty.Register(
            "AdditionalHeader", typeof (string), typeof (WSUIHeaderControl), new PropertyMetadata(default(string)));

        public string AdditionalHeader
        {
            get { return (string) GetValue(AdditionalHeaderProperty); }
            set { SetValue(AdditionalHeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderFontWeightProperty = DependencyProperty.Register(
            "HeaderFontWeight", typeof (FontWeight), typeof (WSUIHeaderControl), new PropertyMetadata(default(FontWeight)));

        public FontWeight HeaderFontWeight
        {
            get { return (FontWeight) GetValue(HeaderFontWeightProperty); }
            set { SetValue(HeaderFontWeightProperty, value); }
        }

    }
}