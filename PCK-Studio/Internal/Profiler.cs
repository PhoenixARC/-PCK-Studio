using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Internal
{
    internal static class Profiler
    {
        private static Stopwatch _stopwatch = new Stopwatch();
        private static TraceListener _listener;

        [Conditional("DEBUG")]
        internal static void Configure(TraceListener listener)
        {
            _listener = listener;
        }

        [Conditional("DEBUG")]
        internal static void Start([CallerMemberName] string caller = default!, [CallerFilePath] string source = default!, [CallerLineNumber] int line = default!)
        {
            _listener?.WriteLine($"Stopwatch starts", category: nameof(Profiler));
            _listener?.WriteLine($"{source}@{caller}:{line}", category: nameof(Profiler));
            _stopwatch.Restart();
        }

        [Conditional("DEBUG")]
        internal static void Stop([CallerMemberName] string caller = default!, [CallerFilePath] string source = default!, [CallerLineNumber] int line = default!)
        {
            _stopwatch.Stop();
            _listener?.WriteLine($"{caller} took {_stopwatch.ElapsedMilliseconds}ms", category: nameof(Profiler));
        }

    }
}
