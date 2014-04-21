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
            Encoding enc = EncodingTools.GetMostEfficientEncoding(str);
            var bytes = enc != null ? enc.GetBytes(str) : Encoding.Default.GetBytes(str);
            var result = enc.GetString(bytes);//Encoding.UTF8.GetString(bytes);
            return result;
        }

    }
}
