﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OF.Core.Logger;

namespace OF.Module.Service
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
            _imageSettings.Add(".doc", @"pack://application:,,,/OF.Module;component/Images/document-word.png");
            _imageSettings.Add(".docx", @"pack://application:,,,/OF.Module;component/Images/document-word.png");
            _imageSettings.Add(".xls", @"pack://application:,,,/OF.Module;component/Images/document-excel.png");
            _imageSettings.Add(".xlsx", @"pack://application:,,,/OF.Module;component/Images/document-excel.png");
            _imageSettings.Add(".pdf", @"pack://application:,,,/OF.Module;component/Images/document-pdf.png");
            _imageSettings.Add(".ppt", @"pack://application:,,,/OF.Module;component/Images/document-powerpoint.png");
            _imageSettings.Add(".pptx", @"pack://application:,,,/OF.Module;component/Images/document-powerpoint.png");
            _imageSettings.Add(".h", @"pack://application:,,,/OF.Module;component/Images/cpp.png");
            _imageSettings.Add(".hpp", @"pack://application:,,,/OF.Module;component/Images/cpp.png");
            _imageSettings.Add(".cpp", @"pack://application:,,,/OF.Module;component/Images/cpp.png");
            _imageSettings.Add(".java",@"pack://application:,,,/OF.Module;component/Images/java.png");
            _imageSettings.Add(".html",@"pack://application:,,,/OF.Module;component/Images/html.png");
            _imageSettings.Add(".xml", @"pack://application:,,,/OF.Module;component/Images/xml.png");
            _imageSettings.Add(".txt", @"pack://application:,,,/OF.Module;component/Images/txt.png");
            _imageSettings.Add(".jpg", @"pack://application:,,,/OF.Module;component/Images/picture.png");
            _imageSettings.Add(".png", @"pack://application:,,,/OF.Module;component/Images/picture.png");
            _imageSettings.Add(".jpeg", @"pack://application:,,,/OF.Module;component/Images/picture.png");
            _imageSettings.Add(".gif", @"pack://application:,,,/OF.Module;component/Images/picture.png");
            _imageSettings.Add(".rtf", @"pack://application:,,,/OF.Module;component/Images/document-word.png");


            _bitmaps = new Dictionary<string, BitmapImage>();
            _bitmaps.Add(DefaultKey, new BitmapImage(new Uri(@"pack://application:,,,/OF.Module;component/Images/document.png")));
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
    public class OFNameToImageConvert : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var name = (string)value;
                if (string.IsNullOrEmpty(name))
                    return ImageHelper.Instance.GetImage(ImageHelper.DefaultKey);

                var ext = Path.GetExtension(name);
                return ImageHelper.Instance.GetImage(ext);
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(string.Format("String: {0}; Exception: {1}", value, ex.Message));
                return ImageHelper.Instance.GetImage(ImageHelper.DefaultKey);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
