using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSUI.Infrastructure.Service.Rules
{
    public class QuoteRule : BaseRule
    {
        public QuoteRule()
        {
        }

        public override void InitRule()
        {
            base.InitRule();
            Rule = "\"(?<group>[\\w\u0020]*)\"";
            Priority = 1;
        }
    }
}
