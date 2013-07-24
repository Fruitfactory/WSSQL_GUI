﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WSUI.Infrastructure.Service.Helpers
{
    public static class HelperFunctions
    {

        public class MatchInfo
        {
            public string Value { get; set; }
            public int Index { get; set; }
            public int Length { get; set; }
        }

        public const string WordRegex = @"(?<word>\w+)";

        public static MatchCollection GetMatchCollection(string inputSequence)
        {
            if (string.IsNullOrEmpty(inputSequence))
                return null;

            var input = Regex.Escape(inputSequence.Trim());

            MatchCollection col = Regex.Matches(input, WordRegex);
            if (col.Count == 0)
                return null;

            return col;
        }


        public static List<string> GetWordList(string inputSequence)
        {
            var col = GetMatchCollection(inputSequence);
            if (col.Count == 0)
                return null;
            var list = new List<string>();
            for (int i = 0; i < col.Count; i++)
            {
                var item = col[i].Value;
                if(string.IsNullOrEmpty(item))
                    continue;
                list.Add(item);
            }

            return list;
        }

        public static List<MatchInfo> GetMatches(string text,string inputSequence)
        {
            var list = GetWordList(inputSequence);
            if (ReferenceEquals(list, null))
                return null;
            var listMatches = new List<MatchInfo>();
            foreach (var word in list)
            {
                var col = Regex.Matches(text, string.Format(@"({0})", Regex.Escape(word)),RegexOptions.IgnoreCase);
                if(col.Count ==  0)
                    continue;
                for (int i = 0; i < col.Count; i++)
                {
                    var item = col[i];
                    listMatches.Add(new MatchInfo() { Value = item.Value, Index = item.Index, Length = item.Length });
                }
            }

            return listMatches.OrderBy(m => m.Index).ToList();
        }


    }
}