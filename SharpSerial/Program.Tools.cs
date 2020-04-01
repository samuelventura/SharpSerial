using System;
using System.Text;

namespace SharpSerial
{
    static class Stdio
    {
        private static readonly object locker = new object();

        public static void WriteLine(string format, params object[] args)
        {
            lock (locker)
            {
                Console.WriteLine(format, args);
                Console.Out.Flush();
            }
        }

        public static string ReadLine() => Console.ReadLine();
    }

    static class Tools
    {
        public static void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            ExceptionHandler(args.ExceptionObject as Exception);
        }

        public static void ExceptionHandler(Exception ex)
        {
            Try(() => Stdio.WriteLine("!{0}", ex.ToString()));
            Environment.Exit(1);
        }

        public static void AnswerHex(byte[] data)
        {
            var sb = new StringBuilder();
            sb.Append("<");
            foreach (var b in data) sb.Append(b.ToString("X2"));
            var txt = sb.ToString();
            Stdio.WriteLine(txt);
        }

        public static byte[] ParseHex(string text)
        {
            Assert(text.Length % 2 == 1, "Odd length expected for {0}:{1}", text.Length, text);
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
            var parts = line.Split(new char[] { '=' });
            if (parts.Length != 2) throw Make("Expected 2 parts in {0}", Readable(line));
            var propertyName = parts[0];
            var propertyValue = parts[1];
            var property = target.GetType().GetProperty(propertyName);
            if (property == null) throw Make("Property not found {0}", Readable(propertyName));
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
