using System.Windows.Documents;

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