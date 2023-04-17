using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.Extentions
{
    internal static class StringExtension
    {
        public static bool ContainsAny(this string str, params char[] chars)
        {
            foreach (var c in str)
            {
                if (chars.Contains(c))
                    return true;
            }
            return false;
        }

    }
}
