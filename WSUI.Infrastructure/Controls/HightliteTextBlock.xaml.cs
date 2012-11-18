using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WSUI.Infrastructure.Controls
{
    /// <summary>
    /// Interaction logic for HightliteTextBlock.xaml
    /// </summary>
    public partial class HightliteTextBlock : HightliteTextBlockBase
    {
        public HightliteTextBlock()
        {
            InitializeComponent();
        }

        #region properties

        public static readonly DependencyProperty FontSizeLabelSizeProperty =
            DependencyProperty.Register("FontSizeLabel",
                                        typeof(double),
                                        typeof(HightliteTextBlock),
                                        new FrameworkPropertyMetadata(12.0));


        public double FontSizeLabel
        {
            get { return (double)GetValue(FontSizeLabelSizeProperty); }
            set { SetValue(FontSizeLabelSizeProperty, value); }
        }

        public static readonly DependencyProperty FontStyleLabelProperty =
            DependencyProperty.Register("FontStyleLabel",
                                        typeof(FontStyle),
                                        typeof(HightliteTextBlock),
                                        new FrameworkPropertyMetadata(FontStyles.Normal));


        public FontStyle FontStyleLabel
        {
            get { return (FontStyle)GetValue(FontStyleLabelProperty); }
            set { SetValue(FontStyleLabelProperty, value); }
        }

        public static readonly DependencyProperty FontWeightLabelProperty =
            DependencyProperty.Register("FontWeightLabel",
                                        typeof(FontWeight),
                                        typeof(HightliteTextBlock),
                                        new FrameworkPropertyMetadata(FontWeights.Normal));


        public FontWeight FontWeightLabel
        {
            get { return (FontWeight)GetValue(FontStyleLabelProperty); }
            set { SetValue(FontStyleLabelProperty, value); }
        }

        public static readonly DependencyProperty ForegroundColorProperty =
           DependencyProperty.Register("ForegroundColor",
                                       typeof(Brush),
                                       typeof(HightliteTextBlock),
                                       new FrameworkPropertyMetadata(Brushes.Black));
        public Brush ForegroundColor
        {
            get { return (Brush)GetValue(ForegroundColorProperty); }
            set { SetValue(ForegroundColorProperty, value); }
        }

        #endregion







        protected override void OnRender(DrawingContext drawingContext)
        {
            if (string.IsNullOrEmpty(Hightlight) || string.IsNullOrEmpty(Text))
            {
                textBlock.Text = Text;
                base.OnRender(drawingContext);
                return;
            }

            var textlbock = textBlock;
            MatchCollection mCol = Regex.Matches(Text, string.Format(@"({0})", Regex.Escape(Hightlight)), RegexOptions.IgnoreCase);
            if (mCol.Count == 0)
            {
                textlbock.Text = this.Text;
                base.OnRender(drawingContext);
                return;
            }
            textlbock.Inlines.Clear();
            int last = 0;
            for (int i = 0; i < mCol.Count; i++)
            {
                var m = mCol[i];
                var sub = Text.Substring(last, m.Index - last);
                Run r = new Run(sub);
                textlbock.Inlines.Add(r);
                sub = Text.Substring(m.Index, m.Length);
                r = new Run(sub);
                r.Background = new SolidColorBrush(BackgroundColor);
                textlbock.Inlines.Add(r);
                last += (m.Index + m.Length);
            }
            if (last < Text.Length)
            {
                var temp = Text.Substring(last, Text.Length - last);
                Run run = new Run(temp);
                textlbock.Inlines.Add(run);
            }

            base.OnRender(drawingContext);
        }

    }
}
