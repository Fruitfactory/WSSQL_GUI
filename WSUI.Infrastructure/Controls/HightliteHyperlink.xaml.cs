using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace WSUI.Infrastructure.Controls
{
    /// <summary>
    /// Interaction logic for HightliteHyperlink.xaml
    /// </summary>
    public partial class HightliteHyperlink
    {
        public HightliteHyperlink()
        {
            InitializeComponent();
        }

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
                textlbock.Inlines.Add(GenerateRun(sub));
                sub = Text.Substring(m.Index, m.Length);
                textlbock.Inlines.Add(GenerateRun(sub, true));
                last += (m.Index + m.Length);
            }
            if (last < Text.Length)
            {
                var temp = Text.Substring(last, Text.Length - last);
                textlbock.Inlines.Add(GenerateRun(temp));
            }

            base.OnRender(drawingContext);
        }

        private Run GenerateRun(string text, bool isBold = false)
        {
            Run run = new Run(text);
            run.FontStyle = FontStyleLabel;
            run.Foreground = ForegroundColor;
            if (isBold)
            {
                run.FontWeight = FontWeights.ExtraBold;
            }
            return run;
        }

    }
}
