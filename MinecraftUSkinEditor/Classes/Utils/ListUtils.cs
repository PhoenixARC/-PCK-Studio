﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Forms.Utilities
{
    public static class ListUtils
    {
        public static IList<T> Swap<T>(this IList<T> list, int index1, int index2)
        {
            T temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
            return list;
        }
    }
}
