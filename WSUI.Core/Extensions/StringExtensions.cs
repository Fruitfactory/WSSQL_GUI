using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSUI.Core.Helpers.DetectEncoding;

namespace WSUI.Core.Extensions
{
    public static class StringExtensions
    {
        private const string IsoEncodingName = "ISO-8859-1";
        private static Encoding encoding = null;

        public static string ConvertToIso(this string str)
        {
            var bytes = Encoding.Default.GetBytes(str);
            Encoding enc = null;
            var result = (enc = Encoding.GetEncoding(IsoEncodingName)) != null ? enc.GetString(bytes) : Encoding.UTF8.GetString(bytes);
            return result;
        }

        public static string ConvertToMostEfficientEncoding(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            encoding = encoding ?? EncodingTools.GetMostEfficientEncoding(str);
            var bytes = encoding != null ? encoding.GetBytes(str) : Encoding.Default.GetBytes(str);
            var result = encoding.GetString(bytes);//Encoding.UTF8.GetString(bytes);
            return result;
        }

    }
}
