/* Copyright (c) 2023-present miku-666
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1.The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PckStudio.Internal.App
{
    internal static class Profiler
    {
        private static Stopwatch _stopwatch = new Stopwatch();

        [Conditional("DEBUG")]
        internal static void Start([CallerMemberName] string caller = default!, [CallerFilePath] string source = default!, [CallerLineNumber] int line = default!)
        {
            Debug.WriteLine($"Stopwatch starts", category: nameof(Profiler));
            Debug.WriteLine($"{source}@{caller}:{line}", category: nameof(Profiler));
            _stopwatch.Restart();
        }

        [Conditional("DEBUG")]
        internal static void Stop([CallerMemberName] string caller = default!, [CallerFilePath] string source = default!, [CallerLineNumber] int line = default!)
        {
            _stopwatch.Stop();
            Debug.WriteLine($"{caller} took {_stopwatch.ElapsedMilliseconds}ms", category: nameof(Profiler));
        }

    }
}
