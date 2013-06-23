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
        private Dictionary<string, string> _imageSettings = null; 


        private ImageHelper()
        {}

        public static ImageHelper Instance
        {
            get { return _instance.Value; }
        }


        protected void Init()
        {
            _imageSettings = new Dictionary<string, string>();
            _imageSettings.Add(".doc", @"pack://application:,,,/WSUI.Module;component/Images/document-word.png");
            _imageSettings.Add(".docx", @"pack://application:,,,/WSUI.Module;component/Images/document-word.png");
            _imageSettings.Add(".xls", @"pack://application:,,,/WSUI.Module;component/Images/document-excel.png");
            _imageSettings.Add(".xlsx", @"pack://application:,,,/WSUI.Module;component/Images/document-excel.png");
            _imageSettings.Add(".pdf", @"pack://application:,,,/WSUI.Module;component/Images/document-pdf.png");
            _imageSettings.Add(".ppt", @"pack://application:,,,/WSUI.Module;component/Images/document-powerpoint.png");
            _imageSettings.Add(".pptx", @"pack://application:,,,/WSUI.Module;component/Images/document-powerpoint.png");
            _imageSettings.Add(".h", @"pack://application:,,,/WSUI.Module;component/Images/cpp.png");
            _imageSettings.Add(".hpp", @"pack://application:,,,/WSUI.Module;component/Images/cpp.png");
            _imageSettings.Add(".cpp", @"pack://application:,,,/WSUI.Module;component/Images/cpp.png");
            _imageSettings.Add(".java",@"pack://application:,,,/WSUI.Module;component/Images/java.png");
            _imageSettings.Add(".html",@"pack://application:,,,/WSUI.Module;component/Images/html.png");
            _imageSettings.Add(".xml", @"pack://application:,,,/WSUI.Module;component/Images/xml.png");
            _imageSettings.Add(".txt", @"pack://application:,,,/WSUI.Module;component/Images/txt.png");
            _bitmaps = new Dictionary<string, BitmapImage>();
            _bitmaps.Add(DefaultKey, new BitmapImage(new Uri(@"pack://application:,,,/WSUI.Module;component/Images/document.png")));
        }

        public BitmapImage GetImage(string fileext)
        {
            fileext = fileext.ToLowerInvariant();
            if (string.IsNullOrEmpty(fileext))
                return _bitmaps[DefaultKey];
            if (_bitmaps.ContainsKey(fileext))
            {
                return _bitmaps[fileext];
            }
            if (_imageSettings.ContainsKey(fileext))
            {
                _bitmaps[fileext] = new BitmapImage(new Uri(_imageSettings[fileext]));
                return _bitmaps[fileext];
            }
            return _bitmaps[DefaultKey];
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
