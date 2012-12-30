using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WSUI.Module.Service
{
    
    public class ImageHelper
    {
        public const string DefaultKey = "Default";
        private static readonly Lazy<ImageHelper> _instance = new Lazy<ImageHelper>(() =>
                                                                               {
                                                                                   var obj = new ImageHelper();
                                                                                   obj.Init();
                                                                                   return obj;
                                                                               });

        private Dictionary<string, BitmapImage> _bitmaps = null;


        private ImageHelper()
        {}

        public static ImageHelper Instance
        {
            get { return _instance.Value; }
        }


        protected void Init()
        {
            _bitmaps = new Dictionary<string, BitmapImage>();
            _bitmaps.Add(".doc", new BitmapImage(new Uri(@"pack://application:,,,/WSUI.Module;component/Images/document-word.png")));
            _bitmaps.Add(".docx", new BitmapImage(new Uri(@"pack://application:,,,/WSUI.Module;component/Images/document-word.png")));
            _bitmaps.Add(".xls", new BitmapImage(new Uri(@"pack://application:,,,/WSUI.Module;component/Images/document-excel.png")));
            _bitmaps.Add(".xlsx", new BitmapImage(new Uri(@"pack://application:,,,/WSUI.Module;component/Images/document-excel.png")));
            _bitmaps.Add(".pdf", new BitmapImage(new Uri(@"pack://application:,,,/WSUI.Module;component/Images/document-pdf.png")));
            _bitmaps.Add(".ppt", new BitmapImage(new Uri(@"pack://application:,,,/WSUI.Module;component/Images/document-powerpoint.png")));
            _bitmaps.Add(".pptx", new BitmapImage(new Uri(@"pack://application:,,,/WSUI.Module;component/Images/document-powerpoint.png")));
            _bitmaps.Add(".h", new BitmapImage(new Uri(@"pack://application:,,,/WSUI.Module;component/Images/cpp.png")));
            _bitmaps.Add(".hpp", new BitmapImage(new Uri(@"pack://application:,,,/WSUI.Module;component/Images/cpp.png")));
            _bitmaps.Add(".cpp", new BitmapImage(new Uri(@"pack://application:,,,/WSUI.Module;component/Images/cpp.png")));
            _bitmaps.Add(".java", new BitmapImage(new Uri(@"pack://application:,,,/WSUI.Module;component/Images/java.png")));
            _bitmaps.Add(".html", new BitmapImage(new Uri(@"pack://application:,,,/WSUI.Module;component/Images/html.png")));
            _bitmaps.Add(".xml", new BitmapImage(new Uri(@"pack://application:,,,/WSUI.Module;component/Images/xml.png")));
            _bitmaps.Add(".txt", new BitmapImage(new Uri(@"pack://application:,,,/WSUI.Module;component/Images/txt.png")));

            _bitmaps.Add(DefaultKey, new BitmapImage(new Uri(@"pack://application:,,,/WSUI.Module;component/Images/document.png")));
        }

        public BitmapImage GetImage(string fileext)
        {
            return !string.IsNullOrEmpty(fileext) && _bitmaps.ContainsKey(fileext) ? _bitmaps[fileext] : _bitmaps[DefaultKey];
        }

    }

    [ValueConversion(typeof(string),typeof(ImageSource))]
    public class NameToImageConvert : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var name = (string)value;
            if (string.IsNullOrEmpty(name))
                return ImageHelper.Instance.GetImage(ImageHelper.DefaultKey);

            var ext = Path.GetExtension(name);
            return ImageHelper.Instance.GetImage(ext);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
