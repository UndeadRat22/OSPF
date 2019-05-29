using System;
using System.Collections.Generic;
using System.Linq;

namespace OSPF
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        public static IEnumerable<int> IndicesWhere<T>(this IEnumerable<T> enumeration, Func<T, bool> predicate)
        {
            return enumeration
                .Select((elem, index) => new { elem, index })
                .Where(indexed => predicate(indexed.elem))
                .Select(indexed => indexed.index);
        }

        public static bool IsEmpty<T>(this IEnumerable<T> enumeration)
        {
            return !enumeration.Any();
        }
    }
}
