using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Core.Extensions
{
    internal static class StringExtensions
    {

        public static string End(this string str, int count)
        {
            return str.Substring(str.Length - count);
        }
    }
}
