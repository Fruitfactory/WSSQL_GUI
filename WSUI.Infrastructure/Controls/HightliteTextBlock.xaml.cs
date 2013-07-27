
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
            base.OnInitialized(e);
            _internalDocumentViewer = internalFlowDocument;
        }    }
}
