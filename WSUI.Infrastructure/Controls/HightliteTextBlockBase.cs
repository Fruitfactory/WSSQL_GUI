using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using WSUI.Infrastructure.Service.Helpers;

namespace WSUI.Infrastructure.Controls
{
    public class HightliteTextBlockBase : ContentControl
    {
        protected WSUIFlowDocumentViewer _internalDocumentViewer;
        protected WSUIFlowDocument _internalDoc;
        protected WSUIParagraph _internalParagraph;

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
            get { return (FontWeight)GetValue(FontWeightLabelProperty); }
            set { SetValue(FontWeightLabelProperty, value); }
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

        protected override void OnInitialized(System.EventArgs e)
        {
            base.OnInitialized(e);
            _internalDoc = new WSUIFlowDocument();
            _internalDoc.FontSize = 0.1;
            _internalDoc.PageWidth = 1000;
            _internalParagraph = new WSUIParagraph();
            _internalParagraph.Margin = new Thickness(0);
            _internalParagraph.Padding = new Thickness(0);
            _internalDoc.Blocks.Add(_internalParagraph);
            _internalDocumentViewer.Document = _internalDoc;    
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            _internalParagraph.Inlines.Clear();
            
            var mCol = HelperFunctions.GetMatches(Text, Hightlight);

            if (string.IsNullOrEmpty(Hightlight) || string.IsNullOrEmpty(Text) || mCol.Count == 0)
            {
                _internalParagraph.Inlines.Add(GenerateRun(Text));
                return;
            }

            int last = 0;
            for (int i = 0; i < mCol.Count; i++)
            {
                var m = mCol[i];
                var sub = Text.Substring(last, m.Index - last);
                _internalParagraph.Inlines.Add(GenerateRun(sub));
                sub = Text.Substring(m.Index, m.Length);
                _internalParagraph.Inlines.Add(GenerateRun(sub, true));
                last = (m.Index + m.Length);
            }
            if (last < Text.Length)
            {
                var temp = Text.Substring(last, Text.Length - last);
                _internalParagraph.Inlines.Add(GenerateRun(temp));
            }
            base.OnRender(drawingContext);
        }


        protected virtual Inline GenerateRun(string text, bool isBold = false)
        {
            WSUIRun run = new WSUIRun(text);
            run.FontStyle = FontStyleLabel;
            run.FontFamily = this.FontFamily;
            run.FontSize = this.FontSizeLabel;
            run.FontWeight = this.FontWeightLabel;
            run.Foreground = this.ForegroundColor;
            if (isBold)
            {
                run.FontWeight = FontWeights.ExtraBold;
            }
            return run;
        }

        protected override void OnPreviewMouseWheel(System.Windows.Input.MouseWheelEventArgs e)
        {
            //e.Handled = true;
        }

    }
}
