﻿using WSUI.Core.Core.Search;
using WSUI.Core.Extensions;
using WSUI.Infrastructure.Implements.Rules;

namespace WSUI.Infrastructure.Implements.Systems
{
    public class ContactEmailSearchSystem : BaseSearchSystem
    {
        public ContactEmailSearchSystem()
        {
            
        }

        public override void Init()
        {
            AddRule(new ContactEmailSearchRule());
            base.Init();
        }

        public override void SetProcessingRecordCount(int first, int second)
        {
            GetRules().ForEach(r => r.SetProcessingRecordCount(first,second));
        }
    }
}