using System;
using System.Collections.Specialized;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace PckStudio.Core.IO.Java
{
    public readonly struct ImportResult<TResult, TStats>(TResult result, TStats stats)
    {
        public readonly TResult Result = result;
        public readonly TStats Stats = stats;
    }
}