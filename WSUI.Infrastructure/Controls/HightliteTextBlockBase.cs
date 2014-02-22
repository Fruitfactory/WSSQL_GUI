using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using WSUI.Core.Extensions;
using WSUI.Infrastructure.Service.Helpers;

namespace WSUI.Infrastructure.Controls
{
  public class HightliteTextBlockBase : ContentControl
  {
    protected HightliteTextBlockBase()
    {
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      Init();
    }

    #region properties

    public static readonly DependencyProperty BackgroundColorProperty =
      DependencyProperty.Register("BackgroundColor", typeof(Color), typeof(HightliteTextBlockBase),
                                  new UIPropertyMetadata(Colors.Yellow, UpdateControlCallBack));

    public Color BackgroundColor
    {
      get
      {
        return (Color)GetValue(BackgroundColorProperty);
      }
      set
      {
        SetValue(BackgroundColorProperty, value);
      }
    }

    public static readonly DependencyProperty TextProperty =
      DependencyProperty.Register("Text", typeof(string), typeof(HightliteTextBlockBase), new UIPropertyMetadata(string.Empty, UpdateControlCallBack));

    public string Text
    {
      get
      {
        return (string)GetValue(TextProperty);
      }
      set
      {
        SetValue(TextProperty, value);
      }
    }

    public static readonly DependencyProperty HightlightProperty =
      DependencyProperty.Register("Hightlight", typeof(string), typeof(HightliteTextBlockBase), new UIPropertyMetadata(string.Empty));

    public string Hightlight
    {
      get
      {
        return (string)GetValue(HightlightProperty);
      }
      set
      {
        SetValue(HightlightProperty, value);
      }
    }

    #endregion properties

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
      get
      {
        return (double)GetValue(FontSizeLabelSizeProperty);
      }
      set
      {
        SetValue(FontSizeLabelSizeProperty, value);
      }
    }

    public static readonly DependencyProperty FontStyleLabelProperty =
      DependencyProperty.Register("FontStyleLabel",
                                  typeof(FontStyle),
                                  typeof(HightliteTextBlockBase),
                                  new FrameworkPropertyMetadata(FontStyles.Normal));

    public FontStyle FontStyleLabel
    {
      get
      {
        return (FontStyle)GetValue(FontStyleLabelProperty);
      }
      set
      {
        SetValue(FontStyleLabelProperty, value);
      }
    }

    public static readonly DependencyProperty FontWeightLabelProperty =
      DependencyProperty.Register("FontWeightLabel",
                                  typeof(FontWeight),
                                  typeof(HightliteTextBlockBase),
                                  new FrameworkPropertyMetadata(FontWeights.Normal));

    public FontWeight FontWeightLabel
    {
      get
      {
        return (FontWeight)GetValue(FontStyleLabelProperty);
      }
      set
      {
        SetValue(FontStyleLabelProperty, value);
      }
    }

    public static readonly DependencyProperty ForegroundColorProperty =
      DependencyProperty.Register("ForegroundColor",
                                  typeof(Brush),
                                  typeof(HightliteTextBlockBase),
                                  new FrameworkPropertyMetadata(Brushes.Black, UpdateControlCallBack));

    public Brush ForegroundColor
    {
      get
      {
        return (Brush)GetValue(ForegroundColorProperty);
      }
      set
      {
        SetValue(ForegroundColorProperty, value);
      }
    }

    #endregion properties

    protected virtual void Init()
    {
    }

    protected virtual void ClearInlines()
    {
    }

    protected virtual void AddInline(Run subText)
    {
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      if (string.IsNullOrEmpty(Hightlight) || string.IsNullOrEmpty(Text))
      {
        ClearInlines();
        AddInline(GenerateRun(Text.ConvertToIso()));
        base.OnRender(drawingContext);
        return;
      }
      Text = Text.ConvertToIso();

      var mCol = HelperFunctions.GetMatches(Text, Hightlight); //Regex.Matches(Text, string.Format(@"({0})", Regex.Escape(Hightlight)), RegexOptions.IgnoreCase);
      if (mCol.Count == 0)
      {
        ClearInlines();
        AddInline(GenerateRun(Text.ConvertToIso()));
        base.OnRender(drawingContext);
        return;
      }

      ClearInlines();
      int last = 0;
      for (int i = 0; i < mCol.Count; i++)
      {
        var m = mCol[i];
        string sub;
        if (m.Index - last < 0)
        {
          sub = Text.Substring(m.Index, m.Length);
          AddInline(GenerateRun(sub));
        }
        else
        {
          sub = Text.Substring(last, m.Index - last);
          AddInline(GenerateRun(sub));
        }
        sub = Text.Substring(m.Index, m.Length);
        AddInline(GenerateRun(sub, true));
        last = (m.Index + m.Length);
      }
      if (last < Text.Length)
      {
        var temp = Text.Substring(last, Text.Length - last);
        AddInline(GenerateRun(temp));
      }

      base.OnRender(drawingContext);
    }

    private Run GenerateRun(string text, bool isBold = false)
    {
      Run run = new Run(text);
      run.FontStyle = FontStyleLabel;
      if (isBold)
      {
        run.FontWeight = FontWeights.ExtraBold;
      }
      return run;
    }
  }
}