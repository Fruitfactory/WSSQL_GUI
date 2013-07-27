using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using WSUI.Infrastructure.Service.Helpers;

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

        protected override void OnInitialized(System.EventArgs e)
        {
            base.OnInitialized(e);
            _internalDocumentViewer = internalFlowDocument;
        }
        protected override Inline GenerateRun(string text, bool isBold = false)
        {
            var run =  base.GenerateRun(text, isBold);
            return new Hyperlink(run);
        }
    }
}
