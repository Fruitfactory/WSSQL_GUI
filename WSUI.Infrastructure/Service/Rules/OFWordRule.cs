using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OF.Infrastructure.Service.Rules
{
    public class OFWordRule : OFBaseRule
    {
        public OFWordRule(){}


        public override void InitRule()
        {
            base.InitRule();
            Rule = @"(?<group>\w+)";
            Priority = 2;
        }
    }
}
