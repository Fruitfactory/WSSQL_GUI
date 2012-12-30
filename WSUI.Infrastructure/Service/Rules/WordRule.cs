using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSUI.Infrastructure.Service.Rules
{
    public class WordRule : BaseRule
    {
        public WordRule(){}


        public override void InitRule()
        {
            base.InitRule();
            Rule = @"(?<group>\w*)";
            Priority = 2;
        }
    }
}
