using System;

namespace WSUI.Infrastructure.Service
{
    public static class ExtensionMethods
    {
         
        public static DateTime GetMinDate(this DateTime date, DateTime next)
        {
            return date > next ? next : date;
        }

    }
}