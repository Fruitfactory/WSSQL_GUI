using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WSUI.Infrastructure.Attributes;
using WSUI.Module.Interface;

namespace WSUI.Module.Service
{
    public class ContactHelpers
    {
        private static readonly string QueryByAddress =
          " OR (System.Kind = 'email' AND CONTAINS(System.Message.FromAddress,'\"{0}*\" OR \"*{0}*\"'))"; 
        private static readonly string QueryContactWhere =
            " (System.Kind = 'contact' AND {1}( CONTAINS(System.Contact.FirstName,'\"{0}*\" OR \"*{0}*\"') OR CONTAINS(System.Contact.LastName,'\"{0}*\" OR \"*{0}*\"') ){2}"; 

        private static readonly string QueryTemplate = "SELECT TOP {0} System.ItemName, System.Contact.FirstName, System.Contact.LastName,System.Contact.EmailAddress,System.Contact.EmailAddress2,System.Contact.EmailAddress3, System.Subject,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Kind,System.Message.FromAddress, System.DateCreated  FROM SystemIndex WHERE ";
        private static readonly string QueryAnd = " OR ( CONTAINS(System.Contact.FirstName,'\"{0}*\" OR \"*{0}*\"') OR CONTAINS(System.Contact.LastName,'\"{0}*\" OR \"*{0}*\"') ))) ";

        private const string EmailPattern = @"\b[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,4}\b";

        public static  string GetContactQuery(string searchCriteria, string lastDate,int top = 5)
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
                    return string.Format("{0}{1}", QueryTemplate, where1) + string.Format(QueryByAddress, arr[0]);
                }
                var address = new StringBuilder(string.Format(QueryByAddress, arr[0]));
                var where2 = string.Format(QueryContactWhere, arr[0], "(", "");
                res += QueryTemplate + where2;
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
            string fromAddress = string.Empty;
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
        [FieldIndex(0)]
        public string ItemName { get; set; }
        [FieldIndex(1)]
        public string FirstName { get; set; }
        [FieldIndex(2)]
        public string LastName { get; set; }
        [FieldIndex(3)]
        public string EmailAddress { get; set; }
        [FieldIndex(4)]
        public string EmailAddress2 { get; set; }
        [FieldIndex(5)]
        public string EmailAddress3 { get; set; }
        [FieldIndex(6)]
        public string Subject { get; set; }
        [FieldIndex(7)]
        public string ItemUrl { get; set; }
        [FieldIndex(8)]
        public string[] ToAddress { get; set; }
        [FieldIndex(9)]
        public DateTime DateReceived { get; set; }
        [FieldIndex(10)]
        public string[] Kind { get; set; }
        [FieldIndex(11)]
        public string[] FromAddress { get; set; }
        [FieldIndex(12)]
        public DateTime DateCreated { get; set; }
    }



}