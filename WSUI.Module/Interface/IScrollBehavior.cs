﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSUI.Infrastructure.Service;

namespace WSUI.Module.Interface
{
    public interface IScrollBehavior
    {
        int CountFirstProcess { get; set; }
        int CountSecondProcess { get; set; }
        int LimitReaction { get; set; }
        event Action SearchGo;
        void NeedSearch(ScrollData sd);
    }
}