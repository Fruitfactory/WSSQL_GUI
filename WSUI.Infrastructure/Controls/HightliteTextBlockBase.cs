using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace WSUI.Infrastructure.Controls
{

    public class HightliteTextBlockBase : ContentControl
    {

        #region properties

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof (Color), typeof (HightliteTextBlockBase),
                                        new UIPropertyMetadata(Colors.Yellow, UpdateControlCallBack));

        public Color BackgroundColor
        {
            get { return (Color) GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }


        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(HightliteTextBlockBase), new UIPropertyMetadata(string.Empty, UpdateControlCallBack));

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty HightlightProperty =
            DependencyProperty.Register("Hightlight", typeof(string), typeof(HightliteTextBlockBase), new UIPropertyMetadata(string.Empty));

        public string Hightlight
        {
            get { return (string)GetValue(HightlightProperty); }
            set { SetValue(HightlightProperty, value); }
        }


        #endregion

        private static void UpdateControlCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HightliteTextBlockBase obj = d as HightliteTextBlockBase;
            obj.InvalidateVisual();
        }

        #region properties

        public static readonly DependencyProperty FontSizeLabelSizeProperty =
            DependencyProperty.Register("FontSizeLabel",
                                        typeof(double),
                                        typeof(HightliteTextBlockBase),
                                        new FrameworkPropertyMetadata(12.0));


        public double FontSizeLabel
        {
            get { return (double)GetValue(FontSizeLabelSizeProperty); }
            set { SetValue(FontSizeLabelSizeProperty, value); }
        }

        public static readonly DependencyProperty FontStyleLabelProperty =
            DependencyProperty.Register("FontStyleLabel",
                                        typeof(FontStyle),
                                        typeof(HightliteTextBlockBase),
                                        new FrameworkPropertyMetadata(FontStyles.Normal));


        public FontStyle FontStyleLabel
        {
            get { return (FontStyle)GetValue(FontStyleLabelProperty); }
            set { SetValue(FontStyleLabelProperty, value); }
        }

        public static readonly DependencyProperty FontWeightLabelProperty =
            DependencyProperty.Register("FontWeightLabel",
                                        typeof(FontWeight),
                                        typeof(HightliteTextBlockBase),
                                        new FrameworkPropertyMetadata(FontWeights.Normal));


        public FontWeight FontWeightLabel
        {
            get { return (FontWeight)GetValue(FontStyleLabelProperty); }
            set { SetValue(FontStyleLabelProperty, value); }
        }

        public static readonly DependencyProperty ForegroundColorProperty =
           DependencyProperty.Register("ForegroundColor",
                                       typeof(Brush),
                                       typeof(HightliteTextBlockBase),
                                       new FrameworkPropertyMetadata(Brushes.Black, UpdateControlCallBack));
        public Brush ForegroundColor
        {
            get { return (Brush)GetValue(ForegroundColorProperty); }
            set { SetValue(ForegroundColorProperty, value); }
        }

        #endregion

    }
}
