using System;
using OF.Infrastructure.Service;
using OF.Module.Interface;
using OF.Module.Interface.Service;

namespace OF.Module.Service
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
