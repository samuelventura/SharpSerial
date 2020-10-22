using System;
using System.Diagnostics;

namespace SharpSerial
{
    public static class Stdio
    {
        private static bool traceTimed = false;
        private static bool traceEnabled = false;
        private static readonly object locker = new object();

        public static string ReadLine() => Console.ReadLine();

        public static void WriteLine(string format, params object[] args)
        {
            lock (locker)
            {
                Console.Out.WriteLine(format, args);
                Console.Out.Flush();
            }
        }

        [Conditional("DEBUG")]
        public static void Trace(string format, params object[] args)
        {
            if (traceEnabled)
            {
                lock (locker)
                {
                    if (traceTimed)
                    {
                        Console.Error.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
                        Console.Error.Write(" ");
                    }
                    Console.Error.WriteLine(format, args);
                    Console.Error.Flush();
                }
            }
        }

        public static void EnableTrace(bool enable, bool timed)
        {
            traceTimed = timed;
            traceEnabled = enable;
        }
    }
}
