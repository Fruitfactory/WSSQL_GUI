using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSUI.Core.Extensions;

namespace WSUI.Infrastructure.Implements.Rules.Helpers
{
    internal class CriteriaHelpers
    {
        #region [instance]

        private static Lazy<CriteriaHelpers> _instance = new Lazy<CriteriaHelpers>(() => new CriteriaHelpers());

        public static CriteriaHelpers Instance
        {
            get { return _instance.Value; }
        }

        #endregion


        #region [methods]

        public string GetFieldCriteriaForEmail(string email)
        {
            if (!email.IsEmail())
                return string.Empty;
            var parts = email.SplitEmail();
            var criteria = BuildCriteriaFromParts(parts.Item1);
            return string.Format(" {0} AND \"{1}*\" ", criteria, parts.Item2);
        }

        public string GetFieldCriteriaForName(string name, string email)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                return string.Empty;
            Tuple<string[],string> emailsParts = email.SplitEmail();
            var criteria = GetFieldCriteriaForName(name);
            return emailsParts != null ? string.Format(" {0} AND \"{1}*\" ", criteria, emailsParts.Item2) : criteria;
        }

        public string GetFieldCriteriaForName(string name)
        {
            var parts = name.SplitString();
            var criteria = BuildCriteriaFromParts(parts);
            return criteria;
        }


        private string BuildCriteriaFromParts(string[] parts)
        {
            if (parts == null)
                return string.Empty;
            var builder = new StringBuilder();
            if (parts.Length > 0)
            {
                builder.AppendFormat(" \"{0}*\" ", parts[0]);
                foreach (var item in parts.Skip(1))
                {
                    builder.AppendFormat(" AND \"{0}*\" ", item);
                }
            }
            return builder.ToString();
        }

        #endregion


    }
}
