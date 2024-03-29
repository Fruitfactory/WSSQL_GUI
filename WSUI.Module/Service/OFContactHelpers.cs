﻿using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using OF.Infrastructure.Attributes;
using OF.Module.Interface;
using OF.Module.Interface.Service;

namespace OF.Module.Service
{
    public class OFContactHelpers
    {
        private static readonly string QueryByAddress =
          " OR (System.Kind = 'email' AND (CONTAINS(System.Message.FromAddress,'\"{0}*\"') OR CONTAINS(System.Message.CcAddress,'\"{0}*\"') OR CONTAINS(System.Message.ToAddress,'\"{0}*\"') OR CONTAINS(System.Search.Contents,'\"{0}*\"'))) "; 
        private static readonly string QueryContactWhere =
            " (System.Kind = 'contact' AND {1}( CONTAINS(System.Contact.FirstName,'\"{0}*\" OR \"*{0}*\"') OR CONTAINS(System.Contact.LastName,'\"{0}*\" OR \"*{0}*\"') ){2}";

        private static readonly string QueryTemplate = "SELECT TOP {0} System.ItemName, System.Contact.FirstName, System.Contact.LastName,System.Contact.EmailAddress,System.Contact.EmailAddress2,System.Contact.EmailAddress3, System.Subject,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Kind,System.Message.FromAddress, System.DateCreated,System.Message.CcAddress  FROM SystemIndex WHERE ";
        private static readonly string QueryAnd = " OR ( CONTAINS(System.Contact.FirstName,'\"{0}*\" OR \"*{0}*\"') OR CONTAINS(System.Contact.LastName,'\"{0}*\" OR \"*{0}*\"') ))) ";

        private const string EmailPattern = @"\b[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,4}\b";

        public static string GetContactQuery(string searchCriteria, string lastDate,int top = 5)
        {
            searchCriteria = searchCriteria.Trim();
            string res = String.Empty;

            string queryContactTemplate = string.Format(QueryTemplate, top);

            if (searchCriteria.IndexOf(' ') > -1)
            {
                StringBuilder strBuilder = new StringBuilder();
                var arr = searchCriteria.Split(' ').ToList();
                if (arr == null || arr.Count == 1)
                {
                    var where1 = string.Format(QueryContactWhere, arr[0], "", ")");
                    return string.Format("{0}{1}", queryContactTemplate, where1) + string.Format(QueryByAddress, arr[0]);
                }
                var address = new StringBuilder(string.Format(QueryByAddress, arr[0]));
                var where2 = string.Format(QueryContactWhere, arr[0], "(", "");
                res += queryContactTemplate + where2;
                for (int i = 1; i < arr.Count; i++)
                {
                    strBuilder.Append(string.Format(QueryAnd, arr[i]));
                    address.Append(string.Format(QueryByAddress, arr[i]));
                }
                res += strBuilder.ToString() + address.ToString();
            }
            else
            {
                var where = string.Format(QueryContactWhere, searchCriteria, "", ")");
                res = string.Format("{0}{1}", queryContactTemplate, where) + string.Format(QueryByAddress, searchCriteria);
            }
            res += string.Format(" AND System.Message.DateReceived < '{0}'  ORDER BY System.DateCreated DESC", lastDate);
            return res;
        }

        public static string GetEmailAddress(string[] from, string searchCriteria)
        {
            string fromAddress = null;
            if (from != null)
            {
                var arr = searchCriteria.Trim().Split(' ');
                if (arr != null && arr.Length > 0)
                {
                    foreach (var s in arr)
                    {
                        fromAddress =
                        from.FirstOrDefault(
                            str =>
                            str.IndexOf(s, StringComparison.CurrentCultureIgnoreCase) > -1 &&
                            Regex.IsMatch(str, EmailPattern, RegexOptions.IgnoreCase));
                        if (!string.IsNullOrEmpty(fromAddress))
                            break;
                    }
                }
                else
                    fromAddress =
                        from.FirstOrDefault(
                            str =>
                            str.IndexOf(searchCriteria.Trim(), StringComparison.CurrentCultureIgnoreCase) > -1 &&
                            Regex.IsMatch(str, EmailPattern, RegexOptions.IgnoreCase));
            }
            return fromAddress;
        }


    }

    public class ContactItem : ISearchData
    {
        [OFFieldIndex(0)]
        public string ItemName { get; set; }
        [OFFieldIndex(1)]
        public string FirstName { get; set; }
        [OFFieldIndex(2)]
        public string LastName { get; set; }
        [OFFieldIndex(3)]
        public string EmailAddress { get; set; }
        [OFFieldIndex(4)]
        public string EmailAddress2 { get; set; }
        [OFFieldIndex(5)]
        public string EmailAddress3 { get; set; }
        [OFFieldIndex(6)]
        public string Subject { get; set; }
        [OFFieldIndex(7)]
        public string ItemUrl { get; set; }
        [OFFieldIndex(8)]
        public string[] ToAddress { get; set; }
        [OFFieldIndex(9)]
        public DateTime DateReceived { get; set; }
        [OFFieldIndex(10)]
        public string[] Kind { get; set; }
        [OFFieldIndex(11)]
        public string[] FromAddress { get; set; }
        [OFFieldIndex(12)]
        public DateTime DateCreated { get; set; }
        [OFFieldIndex(13)]
        public string[] CcAddress { get; set; }

    }



}