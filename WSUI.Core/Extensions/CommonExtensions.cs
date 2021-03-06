﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OF.Core.Extensions
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

        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }

        public static void AddRange<T>(this IList<T> col, IList<T> items)
        {
            if (col == null || items == null)
                return;
            foreach (var item in items)
                col.Add(item);
        }

        public static bool IsNull(this object arg)
        {
            return ReferenceEquals(arg, null);
        }

        public static bool IsNotNull(this object arg)
        {
            return !ReferenceEquals(arg, null);
        }

        public static bool IsOneExist<T>(this IEnumerable<T> list)
        {
            return list.IsNotNull() && list.Count() == 1;
        }

        public static bool IsNotEmpty<T>(this IEnumerable<T> list)
        {
            return list.IsNotNull() && list.Count() != 0;
        }

        public static bool IsEmpty<T>(this IEnumerable<T> list)
        {
            return list.IsNotNull() && !list.Any();
        }


        public static bool IsStringEmptyOrNull(this object obj)
        {
            return obj is string && string.IsNullOrEmpty(obj as string);
        }

        public static int GetErrorCode(this int hresult)
        {
            return hresult & 0x0000FFFF;
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int size)
        {
            while (list.Any())
            {
                yield return list.Take(size);
                list = list.Skip(size);
            }
        }

    }
}