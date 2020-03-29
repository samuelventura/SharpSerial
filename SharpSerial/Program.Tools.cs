using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace SharpSerial
{
    static class Tools
    {
        public static void Try(Action action, Action<Exception> handler = null)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                if (handler != null)
                {
                    Try(() => { handler(ex); });
                }
            }
        }

        public static Exception Make(string format, params object[] args)
        {
            var line = format;
            if (args.Length > 0) line = string.Format(format, args);
            return new Exception(line);
        }

        public static string Relative(string folder)
        {
            var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(root, folder);
        }

        public static void Dump(Exception ex)
        {
            var folder = Relative("Exceptions");
            Directory.CreateDirectory(folder);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
            var filename = string.Format("Exception-{0}-SharpSerial-{1:000000}.txt", timestamp, Process.GetCurrentProcess().Id);
            var path = Path.Combine(folder, filename);
            File.WriteAllText(path, ex.ToString());
        }

        public static string Readable(string text)
        {
            var sb = new StringBuilder();
            foreach (var c in text)
            {
                if (Char.IsControl(c)) sb.Append(((int)c).ToString("X2"));
                else if (Char.IsWhiteSpace(c)) sb.Append(((int)c).ToString("X2"));
                else sb.Append(c);
            }
            return sb.ToString();
        }

        public static void Assert(bool condition, string format, params object[] args)
        {
            if (!condition) throw Make(format, args);
        }
    }
}
