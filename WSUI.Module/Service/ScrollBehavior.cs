using System;
using WSUI.Infrastructure.Service;
using WSUI.Module.Interface;

namespace WSUI.Module.Service
{
    public class ScrollBehavior : IScrollBehavior
    {
        #region Implementation of IScrollBehavior

        public int CountFirstProcess { get; set; }
        public int CountSecondProcess { get; set; }
        public int LimitReaction { get; set; }
        public event Action SearchGo;

        public void NeedSearch(ScrollData sd)
        {
            var scrollArgs = sd as ScrollData;
            System.Diagnostics.Debug.WriteLine(scrollArgs.ToString());
            var result = scrollArgs.VerticalOffset * 100 / scrollArgs.ScrollableHeight;
            if (result > LimitReaction)
            {
                var temp = SearchGo;
                if(temp != null)
                    temp.Invoke();
            }
        }

        #endregion



    }
}
