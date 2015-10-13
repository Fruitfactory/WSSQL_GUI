using System.Windows.Documents;

namespace OF.Infrastructure.Controls
{
  /// <summary>
  /// Interaction logic for HightliteHyperlink.xaml
  /// </summary>
  public partial class OFHightliteHyperlink
  {
    public OFHightliteHyperlink()
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