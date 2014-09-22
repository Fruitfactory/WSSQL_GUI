﻿using System;
using System.Text;
using WSUI.Core.Helpers.DetectEncoding;

namespace WSUI.Core.Extensions
{
    public static class StringExtensions
    {
        private const string IsoEncodingName = "ISO-8859-1";
        private static Encoding encoding = null;

        private static readonly char[] separators = new char[] { ' ', '.', ',' };
        private readonly static char Apersand = '@';


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

        public static string[] SplitString(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return default(string[]);
            return str.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }

        public static Tuple<string[],string> SplitEmail(this string email)
        {
            if (string.IsNullOrEmpty(email))
                return default(Tuple<string[], string>);
            var part1 = email.Substring(0, email.IndexOf(Apersand));
            var part2 = email.Substring(email.IndexOf(Apersand) + 1);
            var split1 = part1.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            return new Tuple<string[], string>(split1, part2);
        }


    }
}