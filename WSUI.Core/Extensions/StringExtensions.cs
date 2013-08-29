using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

    }
}
