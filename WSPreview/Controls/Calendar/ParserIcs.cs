using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WSPreview.PreviewHandler.Controls.Calendar
{
    static class ParserIcs
    {
        public const string DateFormat = "yyyyMMddTHHmmssZ";
        public static IFormatProvider Format = System.Globalization.CultureInfo.InvariantCulture;


        public static void ParseContent(string content, FileIcsProperty obj)
        {
            if(string.IsNullOrEmpty(content))
                return;

            var properties = obj.GetType().GetProperties().Where(pi => pi.GetCustomAttributes(typeof(ParseAttribute),false).Length > 0);
            foreach (var propertyInfo in properties)
            {
                var attr = (ParseAttribute)propertyInfo.GetCustomAttributes(typeof (ParseAttribute), false)[0];
                if(attr == null)
                    continue;
                switch (attr.Type)
                {
                    case TypePropertyIcs.Line:
                        propertyInfo.SetValue(obj, GetValue(content, attr, propertyInfo.PropertyType), null);
                        break;
                    case TypePropertyIcs.Multiline:
                        propertyInfo.SetValue(obj,GetMultValue(content,attr),null);
                        break;
                }                
            }
        }

        private static object GetValue(string content, ParseAttribute attr, Type type)
        {
            object res = null;
            var match = Regex.Match(content, attr.Begin,RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (match.Groups.Count > 0)
            {
                res = match.Groups[1].Value;
            }
            if (type == typeof(DateTime) && res != null)
            {
                res = DateTime.ParseExact(res.ToString().Replace("\r", "").Replace("\n",""), DateFormat, Format);
            }
            else
                res = Convert.ChangeType(res, type);

            return res;//type == typeof(DateTime) && res != null ? DateTime.ParseExact(res.ToString(),DateFormat,Format) : Convert.ChangeType(res, type);
        }

        private static string GetMultValue(string content, ParseAttribute attr)
        {
            string result = string.Empty;

            var matchB = Regex.Match(content, attr.Begin, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var matchE = Regex.Match(content, attr.End, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (matchB.Index > -1 && matchE.Index > -1)
            {
                var count = matchB.Index + matchB.Length;
                result = content.Substring(count, matchE.Index - count);
            }
            return result;
        }
    }
}
