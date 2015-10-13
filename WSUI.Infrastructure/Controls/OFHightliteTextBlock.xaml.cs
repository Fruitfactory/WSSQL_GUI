using System.Windows.Documents;

namespace OF.Infrastructure.Controls
{
  /// <summary>
  /// Interaction logic for HightliteTextBlock.xaml
  /// </summary>
  public partial class OFHightliteTextBlock : OFHightliteTextBlockBase
  {
      public OFHightliteTextBlock()
    {
      InitializeComponent();
    }

    protected override void ClearInlines()
    {
      textBlock.Inlines.Clear();
    }

    protected override void AddInline(Run subText)
    {
      textBlock.Inlines.Add(subText);
    }
  }
}