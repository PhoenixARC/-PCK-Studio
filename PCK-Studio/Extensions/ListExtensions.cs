using System.Collections.Generic;

namespace PckStudio.Extensions
{
    internal static class ListExtensions
    {
        public static IList<T> Swap<T>(this IList<T> list, int index1, int index2)
        {
            T temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
            return list;
        }
        
        public static bool IndexInRange<T>(this IList<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }
    }
}
