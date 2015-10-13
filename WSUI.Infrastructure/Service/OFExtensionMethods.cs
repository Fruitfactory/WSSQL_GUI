using System;

namespace OF.Infrastructure.Service
{
    public static class OFExtensionMethods
    {
         
        public static DateTime GetMinDate(this DateTime date, DateTime next)
        {
            return date > next ? next : date;
        }

    }
}