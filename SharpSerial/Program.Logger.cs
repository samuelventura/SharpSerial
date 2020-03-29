using System;

namespace SharpSerial
{
    partial class Program
    {
        static void Exception(Exception ex) => Log("E", ex.ToString());
        static void Error(string format, params object[] args) => Log("E", format, args);
        static void Success(string format, params object[] args) => Log("S", format, args);
        static void Warn(string format, params object[] args) => Log("W", format, args);
        static void Info(string format, params object[] args) => Log("I", format, args);
        static void Debug(string format, params object[] args) => Log("D", format, args);
        static void Trace(string format, params object[] args) => Log("T", format, args);

        static void Log(string level, string format, params object[] args)
        {
            var text = format;
            if (args.Length > 0) text = string.Format(format, args);
            foreach (var line in text.Split('\n')) Console.Error.WriteLine("#{0} {1}", level, text);
        }
    }
}
