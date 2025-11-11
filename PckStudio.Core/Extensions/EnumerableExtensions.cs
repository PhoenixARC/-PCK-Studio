using System.Collections.Generic;
using System.Linq;

namespace PckStudio.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<(int index, T value)>enumerate<T>(this IEnumerable<T> array)
        {
            int i = 0;
            foreach (T item in array)
            {
                yield return (i++, item);
            }
            yield break;
        }

        public static bool EqualsAny<T>(this T type, params T[] items)
        {
            foreach (T item in items)
            {
                if (item.Equals(type))
                    return true;
            }
            return false;
        }

        public static bool ContainsAny<T>(this IEnumerable<T> array, params T[] items)
        {
            foreach (T item in array)
            {
                if (items.Contains(item))
                    return true;
            }
            return false;
        }
    }
}
