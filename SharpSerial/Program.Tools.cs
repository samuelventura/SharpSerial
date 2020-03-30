using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace SharpSerial
{
    static class Tools
    {
        public static void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            ExceptionHandler(args.ExceptionObject as Exception);
        }

        public static void ExceptionHandler(Exception ex)
        {
            Tools.Try(() => Console.WriteLine("!{0}", ex.ToString()));
            Environment.Exit(1);
        }

        public static void AnswerHex(byte[] data)
        {
            var sb = new StringBuilder();
            sb.Append("<");
            foreach (var b in data) sb.Append(b.ToString("X2"));
            var txt = sb.ToString();
            Console.WriteLine(txt);
        }

        public static byte[] ParseHex(string text)
        {
            Tools.Assert(text.Length % 2 == 1, "Odd length expected for {0}:{1}", text.Length, text);
            var bytes = new byte[text.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                var b2 = text.Substring(1 + i * 2, 2);
                bytes[i] = Convert.ToByte(b2, 16);
            }
            return bytes;
        }

        public static void SetProperty(object target, string line)
        {
            var parts = line.Split(new char[] { '=' }, 2);
            var propertyName = parts[0];
            var propertyValue = parts[1];
            var property = target.GetType().GetProperty(propertyName);
            var value = Convert.ChangeType(propertyValue, property.PropertyType);
            property.SetValue(target, value, null);
        }

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
