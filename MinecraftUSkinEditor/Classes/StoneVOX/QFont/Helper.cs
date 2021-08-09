using System.Collections.Generic;

namespace QuickFont
{
    class Helper
    {
        public static T[] ToArray<T>(ICollection<T> collection)
        {
            T[] output = new T[collection.Count];
            collection.CopyTo(output, 0);
            return output;
        }
    }
}
