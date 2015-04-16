using System.Windows.Documents;

namespace OF.Infrastructure.Controls
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