using System;
using System.Collections;
using System.Collections.Generic;

namespace WSUI.Core.Extensions
{
    public static class CommonExtensions
    {

        public static void ForEach<T>(this IList<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }

    }
}