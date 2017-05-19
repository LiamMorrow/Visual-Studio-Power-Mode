using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerMode
{
    /// <summary>
    /// Methods for helping with debugging.  All methods will only run in DEBUG mode so safe to include in release builds
    /// </summary>
    static class DebugHelper
    {
        readonly static ConcurrentDictionary<string, Stopwatch> s_stopwatches = new ConcurrentDictionary<string, Stopwatch>();

        /// <summary>
        /// Starts a timer with a unique id
        /// </summary>
        /// <param name="id">A unique id for timing objects</param>
        [Conditional("DEBUG")]
        public static void StartTimer(string id)
        {
            Debug.WriteLine($"{id} started timing");
            s_stopwatches.AddOrUpdate(id, _ => Stopwatch.StartNew(), (_, __) =>
            {
                Debug.WriteLine($"Stopwatch replaced! ID: {id}");
                return Stopwatch.StartNew();
            });
        }

        /// <summary>
        /// Finishes a timer and prints the runtime to debug output
        /// </summary>
        /// <param name="id">A unique id for timing objects</param>
        [Conditional("DEBUG")]
        public static void FinishTimer(string id)
        {
            if (s_stopwatches.TryRemove(id, out var stopwatch))
            {
                stopwatch.Stop();
                Debug.WriteLine($"{id} finished in {stopwatch.Elapsed}");
            }
            else
            {
                Debug.WriteLine($"Unable to get time for id: {id}");
            }
        }
    }
}
