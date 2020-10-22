using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SharpSerial
{
    //this class should not swallow exceptions outside dispose
    public class SerialProcess : ISerialStream, IDisposable
    {
        private readonly Process process;
        private readonly int pid;

        public int Pid { get { return pid; } }

        public SerialProcess(object settings)
        {
            var ss = new SerialSettings(settings);
            var args = new StringBuilder();
            foreach (var p in ss.GetType().GetProperties())
            {
                if (args.Length > 0) args.Append(" ");
                args.AppendFormat("{0}={1}", p.Name, p.GetValue(ss, null).ToString());
            }
            process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                FileName = typeof(SerialProcess).Assembly.Location,
                Arguments = args.ToString(),
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = false,
            };
            EnableStandardError(process.StartInfo);
            process.Start();
            pid = process.Id;
            ForwardStandardError(process.StandardError);
        }

        public void Dispose()
        {
            Tools.Try(() =>
            {
                process.StandardInput.Close();
                process.WaitForExit(200);
            });
            Tools.Try(process.Kill);
            Tools.Try(process.Dispose);
        }

        public void Write(byte[] data)
        {
            WriteHex(data);
            var line = ReadLine();
            Tools.Assert(line == "<ok", "Unexpected write response {0}", line);
        }

        public byte[] Read(int size, int eop, int toms)
        {
            WriteLine("$r,{0},{1},{2}", size, eop, toms);
            return ParseHex(ReadLine());
        }

        private string ReadLine()
        {
            var line = process.StandardOutput.ReadLine();
            if (line == null) throw new EndOfStreamException("Serial process EOF");
            if (line.StartsWith("!"))
            {
                var trace = process.StandardOutput.ReadToEnd();
                throw new SerialException(line.Substring(1), trace);
            }
            return line;
        }

        private void WriteLine(string format, params object[] args)
        {
            process.StandardInput.WriteLine(format, args);
            process.StandardInput.Flush();
        }

        private void WriteHex(byte[] data)
        {
            var sb = new StringBuilder();
            sb.Append(">");
            foreach (var b in data) sb.Append(b.ToString("X2"));
            WriteLine(sb.ToString());
        }

        private byte[] ParseHex(string text)
        {
            Tools.Assert(text.StartsWith("<"), "First char < expected for {0}:{1}", text.Length, text);
            Tools.Assert(text.Length % 2 == 1, "Odd length expected for {0}:{1}", text.Length, text);
            var bytes = new byte[text.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                var b2 = text.Substring(1 + i * 2, 2);
                bytes[i] = Convert.ToByte(b2, 16);
            }
            return bytes;
        }

        [Conditional("DEBUG")]
        private void EnableStandardError(ProcessStartInfo psi)
        {
            psi.RedirectStandardError = true;
        }

        [Conditional("DEBUG")]
        private void ForwardStandardError(StreamReader reader)
        {
            Task.Run(() =>
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    Stdio.Trace(line);
                    line = reader.ReadLine();
                }
            });
        }
    }
}
