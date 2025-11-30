using System;
using System.Collections.Specialized;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace PckStudio.Core.IO.Java
{
    public readonly struct ImportResult<T>(T result, int count)
    {
        public readonly T Result = result;

        private const int STRIPE = sizeof(uint) * 8;
        private readonly BitVector32[] _masks = new BitVector32[count / STRIPE +1];
        private readonly int _count = count;

        public void SetMarked(int i)
        {
            if (i >= _count)
                return;
            int bitVectorIndex = Math.DivRem(i, STRIPE, out int bit);
            _masks[bitVectorIndex][bit] = true;
        }

        // MAGIC
        private static int CountBits(int i)
        {
            i -= ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }

        public bool Success => _masks.All(m => m.Data == -1);
        public int RemainingCount => _masks.Select(m => CountBits(m.Data)).Sum();
        public int ImportCount => _count - RemainingCount;
    }
}