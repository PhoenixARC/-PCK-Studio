using System.Collections.Generic;

namespace PckStudio.Classes.Extentions
{
    internal static class EnumerableExtentions
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
