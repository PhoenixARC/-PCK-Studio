using System.Collections.Generic;
using System.Linq;

namespace PckStudio.Extensions
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<(int index, T type)>enumerate<T>(this IEnumerable<T> array)
        {
            int i = 0;
            foreach (var item in array)
            {
                yield return (i++, item);
            }
            yield break;
        }

        public static bool ContainsAny<T>(this IEnumerable<T> array, params T[] items)
        {
            foreach (var item in array)
            {
                if (items.Contains(item))
                    return true;
            }
            return false;
        }
    }
}
