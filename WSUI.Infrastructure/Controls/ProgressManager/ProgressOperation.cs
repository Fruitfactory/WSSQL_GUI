﻿using System;
using System.Windows;


namespace WSUI.Infrastructure.Controls.ProgressManager
{
    public class ProgressOperation
    {
        public string Caption { get; set; }
        public int DelayTime { get; set; }
        public Action CancelAction { get; set; }

        //public ProgressBarStyle Style { get; set; }
        public int Max { get; set; }
        public int Min { get; set; }
        public bool Canceled { get; set; } // button Cancel
        public Point Location { get; set; }
        public Size Size { get; set; }
    }
}