using System.Collections.Generic;

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
    }
}
