﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using OF.Core.Helpers.DetectEncoding;
using System.Text.RegularExpressions;

namespace OF.Core.Extensions
{
    public static class StringExtensions
    {
        private const string IsoEncodingName = "ISO-8859-1";
        private static Encoding encoding = null;

        private static readonly char[] separators = new char[] { ' ', '.', ',' };
        private readonly static char Apersand = '@';
        
        private const string EmailPattern = @"\b[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,4}\b";
        private const string HtmlCommentPattern = @"<!--[\d\D]*?-->";
        private const string AmountPattern = @"^\$?(\d{1,3}(\,\d{3})*|(\d+))(\.\d{0,2})?$";
        private const string MimeStringFormat = @"(?:=\?)([^\?]+)(?:\?([BQ])\?)([^\?]*)(?:\?=)";


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

        public static string[] SplitEmail(this string email)
        {
            if (string.IsNullOrEmpty(email) || !email.IsEmail())
                return null;
            var part1 = email.Substring(0, email.IndexOf(Apersand));
            var part2 = email.Substring(email.IndexOf(Apersand) + 1);
            var list = new List<string>() {part1,part2  };
            return list.ToArray();
        }

        public static bool IsEmail(this string email)
        {
            return !string.IsNullOrEmpty(email) && Regex.IsMatch(email, EmailPattern, RegexOptions.IgnoreCase);
        }

        public static string ClearString(this string str, string pattern = "['()\"]")
        {
            return string.IsNullOrEmpty(str) ? string.Empty : Regex.Replace(str, pattern, "",RegexOptions.Compiled);
        }

        public static string EkranString(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            if (str.IndexOf('$') > -1)
            {
                return str.Replace("$",@"\$");
            }
            return str;
        }

        public static bool IsHtmlComment(this string str)
        {
            return !string.IsNullOrEmpty(str) && Regex.IsMatch(str, HtmlCommentPattern,RegexOptions.IgnoreCase);
        }

        public static bool IsAmount(this string str)
        {
            return !string.IsNullOrEmpty(str) && Regex.IsMatch(str, AmountPattern, RegexOptions.IgnoreCase);
        }

        public static bool IsEmpty(this string @this)
        {
            return string.IsNullOrEmpty(@this);
        }


        public static string HighlightKeyWords(this string text, string[] keywords, string cssClass, bool fullMatch)
        {
            if (text == String.Empty || keywords == null || keywords.Length == 0 || cssClass == String.Empty)
                return text;
            var words = keywords;
            if (!fullMatch)
                return words.Select(word => word.Trim()).Aggregate(text,
                             (current, pattern) =>
                             Regex.Replace(current,
                                             pattern,
                                               string.Format("<span style=\"background-color:{0}\">{1}</span>",
                                               cssClass,
                                               "$0"),
                                               RegexOptions.IgnoreCase));
            return words.Select(word => "\\b" + word.Trim() + "\\b")
                        .Aggregate(text, (current, pattern) =>
                                  Regex.Replace(current,
                                  pattern,
                                    string.Format("<span style=\"background-color:{0}\">{1}</span>",
                                    cssClass,
                                    "$0"),
                                    RegexOptions.IgnoreCase));

        }

        public static int GetIternalHashCode(this string str)
        {
            int h = 0;
            if (h == 0 && str.Length > 0)
            {
                char[] val = str.ToCharArray();

                for (int i = 0; i < str.Length; i++)
                {
                    h = 31 * h + val[i];
                }
            }
            return h;
        }

        public static bool IsMimeFormat(this string obj)
        {
            return Regex.IsMatch(obj, MimeStringFormat);
        }

        public static string DecodeMimeString(this string obj)
        {
            if (!Regex.IsMatch(obj, MimeStringFormat))
            {
                return obj;
            }
            MatchCollection m = Regex.Matches(obj, MimeStringFormat);
            if (m.Count > 0)
            {
                string charset = m[0].Groups[1].Value;
                string typeEncoding = m[0].Groups[2].Value;
                string data = m[0].Groups[3].Value;
                string res = "";
                switch (typeEncoding)
                {
                    case "B":
                        byte[] b = Convert.FromBase64String(data);
                        res = Encoding.GetEncoding(charset).GetString(b);
                        break;
                    case "Q":
                        res = data.Replace('_', ' ').Replace('=', ' ');
                        break;
                }
                return res;

            }
            return string.Empty;
        }


        public static string ToLowerCase(this object obj)
        {
            if (obj == null)
            {
                return "";
            }
            return obj.ToString().ToLowerInvariant();
        }

    }
}