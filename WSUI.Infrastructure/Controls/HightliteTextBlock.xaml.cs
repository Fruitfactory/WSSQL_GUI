
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

        protected override void OnInitialized(System.EventArgs e)
        {
            _internalDocumentViewer = internalFlowDocument;
            base.OnInitialized(e);
        }    }
}
