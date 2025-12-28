using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PckStudio.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<(int index, T value)>Enumerate<T>(this IEnumerable<T> array)
        {
            int i = 0;
            foreach (T item in array)
            {
                yield return (i++, item);
            }
            yield break;
        }

        public static ImageList ToImageList(this IEnumerable<Image> images)
        {
            ImageList imageList = new ImageList
            {
                ColorDepth = ColorDepth.Depth32Bit
            };
            imageList.Images.AddRange(images.ToArray());

            return imageList;
        }

        public static string ToString<T>(this IEnumerable<T> range, string seperator)
            => range
            .Select(t => t.ToString())
            .Aggregate((res, next) => string.IsNullOrWhiteSpace(next) ? res : res + seperator + next);

        public static bool EqualsAny<T>(this T type, params T[] items) => items.Any(item => item.Equals(type));

        public static bool ContainsAny<T>(this IEnumerable<T> source, params T[] items) => source.Any(t => items.Contains(t));
    }
}
