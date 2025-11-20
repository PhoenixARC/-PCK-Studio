using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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

        public static ImageList ToImageList(this Image[] images)
        {
            ImageList imageList = new ImageList
            {
                ColorDepth = ColorDepth.Depth32Bit
            };
            imageList.Images.AddRange(images);

            return imageList;
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
