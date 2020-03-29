using System;
using System.IO;
using System.Diagnostics;

namespace SharpSerial
{
    class Logger
    {
        public void Exception(Exception ex) => Log("E", ex.ToString());
        public void Error(string format, params object[] args) => Log("E", format, args);
        public void Success(string format, params object[] args) => Log("S", format, args);
        public void Warn(string format, params object[] args) => Log("W", format, args);
        public void Info(string format, params object[] args) => Log("I", format, args);
        public void Debug(string format, params object[] args) => Log("D", format, args);
        public void Trace(string format, params object[] args) => Log("T", format, args);
        public virtual void Log(string level, string format, params object[] args) { }
    }

    class FileLogger : Logger, IDisposable
    {
        private readonly TextWriter writer;

        public FileLogger()
        {
            var ts = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
            var pid = Process.GetCurrentProcess().Id;
            var file = string.Format("SharpSerial_{0}_{1:000000}.txt", ts, pid);
            var folder = Tools.Relative("SharpSerial");
            Directory.CreateDirectory(folder);
            writer = File.CreateText(Path.Combine(folder, file));
        }

        public void Dispose()
        {
            Tools.Try(writer.Dispose);
        }

        public override void Log(string level, string format, params object[] args)
        {
            var text = format;
            if (args.Length > 0) text = string.Format(format, args);
            var ts = DateTime.Now.ToString("HH:mm:ss.fff");
            foreach (var line in text.Split('\n')) writer.WriteLine("{0} {1} {2}", ts, level, text);
            writer.Flush();
        }
    }
}
