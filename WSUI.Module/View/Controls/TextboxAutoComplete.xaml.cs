﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;

namespace OF.Module.View.Controls
{
    /// <summary>
    /// Interaction logic for TextboxAutoComplete.xaml
    /// </summary>
    public partial class TextboxAutoComplete : Canvas,INotifyPropertyChanged
    {
        #region fields

        private VisualCollection _controls;
        private TextBox _textBox;
        private ComboBox _comboBox;

        #endregion

        public TextboxAutoComplete()
        {
            _controls = new VisualCollection(this);
            InitializeComponent();

            _comboBox = new ComboBox();
            //_comboBox.IsSynchronizedWithCurrentItem = true;
            _comboBox.SelectionChanged += (o, e) =>
                                              {
                                                  if (_comboBox.SelectedValue != null)
                                                  {
                                                      _textBox.Text = (string)_comboBox.SelectedValue;
                                                      _textBox.Focus();
                                                  }
                                              };
            _comboBox.KeyDown += (o, e) =>
                                     {
                                         switch (e.Key)
                                         {
                                             case Key.Enter:
                                                 _textBox.Text = _comboBox.SelectedValue as string;
                                                 _comboBox.IsDropDownOpen = false;
                                                 break;
                                         }
                                         //OnKeyDownFired(e);
                                     };

            _textBox = new TextBox();
            _textBox.KeyDown += (o, e) =>
                                    {
                                        switch(e.Key)
                                        {
                                            case Key.Escape:
                                                _comboBox.IsDropDownOpen = false;
                                                break;
                                        }
                                        //OnKeyDownFired(e);
                                    };
            //_textBox.LostFocus += (o, e) => _comboBox.IsDropDownOpen = false;

            Binding binding = new Binding("SearchText") { Source = this, Mode = BindingMode.TwoWay,UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged};
            _textBox.SetBinding(TextBox.TextProperty, binding);
            _textBox.SetValue(TextBoxHelper.WatermarkProperty, "Enter Search Criteria");
            _textBox.SetValue(TextBoxHelper.ClearTextButtonProperty, true);
            _controls.Add(_comboBox);
            _controls.Add(_textBox);
            Canvas.SetZIndex(_textBox,100);
        }

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof (string), typeof (TextboxAutoComplete));

        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set
            {
                SetValue(SearchTextProperty, value);
            }
        }

        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof (ObservableCollection<string>),
                                        typeof (TextboxAutoComplete),
                                        new FrameworkPropertyMetadata(null, DataSourceChanged));

        public ObservableCollection<string> DataSource
        {
            get { return (ObservableCollection<string>)GetValue(DataContextProperty); }
            set{SetValue(DataSourceProperty,value);}
        }
            

        private static void DataSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((TextboxAutoComplete)o).DataSourceChanged(e);
        }

        public void DataSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue == null)
            {
                _comboBox.ItemsSource = null;
                _comboBox.IsDropDownOpen = false;
                return;
            }

            _comboBox.ItemsSource = e.NewValue as ObservableCollection<string>;
            _comboBox.SelectedIndex = -1;
            _comboBox.IsDropDownOpen = _comboBox.HasItems;
        }


        protected override Size ArrangeOverride(Size arrangeSize)
        {
            _textBox.Arrange(new Rect(arrangeSize));
            _comboBox.Arrange(new Rect(arrangeSize));
            return base.ArrangeOverride(arrangeSize);
        }

        protected override Visual GetVisualChild(int index)
        {
            return _controls[index];
        }

        protected override int VisualChildrenCount
        {
            get { return _controls.Count; }
        }
#pragma warning disable 67
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnKeyDownFired(KeyEventArgs e)
        {
            if(!PresentationSource.CurrentSources.OfType<PresentationSource>().Any())
                return;
            KeyEventArgs args = new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.CurrentSources.OfType<PresentationSource>().ElementAt(0), 0, e.Key);
            args.RoutedEvent = UIElement.KeyDownEvent;
            RaiseEvent(args);

        }

    }
}
