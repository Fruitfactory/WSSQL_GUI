using System;
using System.Net;
using System.Text;
using WSUI.Core.Helpers.DetectEncoding;
using System.Text.RegularExpressions;

namespace WSUI.Core.Extensions
{
    public static class StringExtensions
    {
        private const string IsoEncodingName = "ISO-8859-1";
        private static Encoding encoding = null;

        private static readonly char[] separators = new char[] { ' ', '.', ',' };
        private readonly static char Apersand = '@';
        
        private const string EmailPattern = @"\b[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,4}\b";


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
            var result = encoding.GetString(bytes);
            return result;
        }

        public static string DecodeString(this string str)
        {
            if(string.IsNullOrEmpty(str))
                return null;
            var decode = WebUtility.HtmlDecode(str);
            return decode;
        }

        public static string[] SplitString(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return default(string[]);
            return str.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }

        public static Tuple<string[],string> SplitEmail(this string email)
        {
            if (string.IsNullOrEmpty(email) || !email.IsEmail())
                return default(Tuple<string[], string>);
            var part1 = email.Substring(0, email.IndexOf(Apersand));
            var part2 = email.Substring(email.IndexOf(Apersand) + 1);
            var split1 = part1.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            return new Tuple<string[], string>(split1, part2);
        }

        public static bool IsEmail(this string email)
        {
            return !string.IsNullOrEmpty(email) && Regex.IsMatch(email, EmailPattern, RegexOptions.IgnoreCase);
        }

        public static string ClearString(this string str, string pattern = "['\\$()\"]")
        {
            return string.IsNullOrEmpty(str) ? string.Empty : Regex.Replace(str, pattern, "",RegexOptions.Compiled);
        }

    }
}