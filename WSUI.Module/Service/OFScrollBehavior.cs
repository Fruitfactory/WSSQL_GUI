using System;
using OF.Infrastructure.Service;
using OF.Module.Interface;
using OF.Module.Interface.Service;

namespace OF.Module.Service
{
    public class OFScrollBehavior : IScrollBehavior
    {
        #region Implementation of IScrollBehavior

        public int CountFirstProcess { get; set; }
        public int CountSecondProcess { get; set; }
        public int LimitReaction { get; set; }
        public event Action SearchGo;

        public void NeedSearch(OFScrollData sd)
        {
            var scrollArgs = sd as OFScrollData;
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
