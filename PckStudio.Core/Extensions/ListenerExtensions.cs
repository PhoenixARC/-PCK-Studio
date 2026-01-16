using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Core.Extensions
{
    public static class DebugEx
    {
        public static void WriteLine<T>(IEnumerable<T> values)
        {
            foreach (T value in values)
                Debug.WriteLine(value);
        }
    }
}
