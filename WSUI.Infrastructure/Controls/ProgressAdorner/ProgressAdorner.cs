using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using MahApps.Metro.Controls;

namespace WSUI.Infrastructure.Controls.ProgressAdorner
{
    public class ProgressAdorner : Adorner
    {
        #region fields

        private VisualCollection _collection;
        private ProgressRing _ring;
        private Pen _pen;
        private Brush _brush;

        #endregion

        public ProgressAdorner(UIElement adornedElement) : base(adornedElement)
        {
            _collection = new VisualCollection(this);
            _ring = new ProgressRing();
            _ring.IsActive = true;
            _ring.HorizontalAlignment = HorizontalAlignment.Center;
            _ring.VerticalAlignment = VerticalAlignment.Center;
            _collection.Add(_ring);
            _pen = new Pen(Brushes.Black,1);
            _brush = new SolidColorBrush(Colors.Gray);
            _brush.Opacity = 0.75;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _collection[index];
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return _collection.Count;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _ring.Arrange(new Rect(0,0,finalSize.Width,finalSize.Height));
            return finalSize;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawRectangle(_brush, null, new Rect(RenderSize));
        }


    }
}
