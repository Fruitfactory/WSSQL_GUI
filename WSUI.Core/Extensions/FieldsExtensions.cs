using System;
using System.Collections.Generic;
using System.Linq;
using WSUI.Core.Interfaces;

namespace WSUI.Core.Extensions
{
    public static class FieldsExtensions
    {
        public static IList<Tuple<string, int>> GetFieldList(this IFieldCash cach, Type type)
        {
            var listAll = cach.GetFields(type);
            if (listAll == null || listAll.Count == 0)
                return null;
            var result = listAll
                .Select(item => new Tuple<string, int>(item.Item2, item.Item3))
                .OrderBy(tuple => tuple.Item2);
            return result.ToList();
        }
    }
}