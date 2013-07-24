﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSUI.Infrastructure.Service.Rules
{
    public interface IRule
    {
        void InitRule();
        string Rule { get; }
        int Priority { get; }
        string[] ApplyRule(string criteria);
        string ClearCriteriaAccordingRule(string criteria);
    }
}