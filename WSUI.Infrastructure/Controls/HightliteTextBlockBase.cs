using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WSUI.Infrastructure.Controls
{

    public class HightliteTextBlockBase : ContentControl
    {

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
            DependencyProperty.Register("Hightlight", typeof(string), typeof(HightliteTextBlockBase), new UIPropertyMetadata(string.Empty, UpdateControlCallBack));

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
        

    }
}
