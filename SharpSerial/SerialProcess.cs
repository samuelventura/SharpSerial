using System;
using System.Text;
using System.Diagnostics;

namespace SharpSerial
{
    public class SerialProcess : ISerialInterface, IDisposable
    {
        private readonly Process process;

        public SerialProcess(string portName, SerialSettings ss = null)
        {
            var args = new StringBuilder();
            args.AppendFormat("PortName={0}", portName);
            if (ss != null)
            {
                foreach (var p in ss.GetType().GetProperties())
                {
                    args.AppendFormat(" ");
                    args.AppendFormat("{0}={1}", p.Name, p.GetValue(ss, null).ToString());
                }
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
                RedirectStandardError = true,
            };
            process.Start();
        }

        public void Dispose()
        {
            Tools.Try(() =>
            {
                process.StandardInput.WriteLine();
                process.WaitForExit(200);
            });
            Tools.Try(process.Kill);
            Tools.Try(process.Dispose);
        }

        public void Write(byte[] data) => WriteHex(data);

        public byte[] Read(int size, int eop, int toms)
        {
            process.StandardInput.WriteLine("$r,{0},{1},{2}", size, (int)eop, toms);
            return ParseHex(process.StandardOutput.ReadLine());
        }

        private void WriteHex(byte[] data)
        {
            var sb = new StringBuilder();
            sb.Append(">");
            foreach (var b in data) sb.Append(b.ToString("X2"));
            process.StandardInput.WriteLine(sb.ToString());
        }

        private byte[] ParseHex(string text)
        {
            Tools.Assert(text.StartsWith("<"), "First char < expected for {0}:{1}", text.Length, text);
            Tools.Assert(text.Length % 2 == 1, "Odd length expected for {0}:{1}", text.Length, text);
            var bytes = new byte[text.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                var b2 = text.Substring(1 + i * 2, 2);
                bytes[i] = Convert.ToByte(b2, 16);
            }
            return bytes;
        }
    }
}
