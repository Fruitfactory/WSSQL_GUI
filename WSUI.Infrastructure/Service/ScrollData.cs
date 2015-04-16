using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OF.Infrastructure.Service
{
    public class ScrollData
    {
        public double VerticalOffset { get; set; }
        public double ScrollableHeight { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", ScrollableHeight, VerticalOffset);
        }
    }
}
