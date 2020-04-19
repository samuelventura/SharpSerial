using System;
using System.Text;

namespace SharpSerial
{
    partial class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += Tools.ExceptionHandler;

            Stdio.EnableTrace(true, true);

            var settings = new SerialSettings();
            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                Stdio.Trace("#Arg {0} {1}", i, arg);
                Tools.SetProperty(settings, arg);
            }

            using (var serial = new SerialDevice(settings))
            {
                var line = Stdio.ReadLine();
                while (line != null)
                {
                    Stdio.Trace("{0}", line);
                    if (line.StartsWith("$"))
                    {
                        var parts = line.Split(new char[] { ',' });
                        switch (parts[0])
                        {
                            case "$r":
                                if (parts.Length != 4) throw Tools.Make("Expected 4 parts in {0}", Tools.Readable(line));
                                var rSize = ParseInt(line, parts[1], 1);
                                var rEop = ParseInt(line, parts[2], 2);
                                var rToms = ParseInt(line, parts[3], 3);
                                var rData = serial.Read(rSize, rEop, rToms);
                                var response = Tools.StringHex(rData);
                                Stdio.Trace(response);
                                Stdio.WriteLine(response);
                                break;
                            default:
                                throw Tools.Make("Unknown command {0}", Tools.Readable(line));
                        }
                    }
                    else if (line.StartsWith(">"))
                    {
                        serial.Write(Tools.ParseHex(line));
                        Stdio.Trace("<ok");
                        Stdio.WriteLine("<ok");
                    }
                    else
                    {
                        throw Tools.Make("Unknown command {0}", Tools.Readable(line));
                    }
                    line = Stdio.ReadLine();
                }
            }
            Environment.Exit(0);
        }

        static int ParseInt(string line, string part, int index)
        {
            if (int.TryParse(part, out var value)) return value;
            throw Tools.Make("Invalid int at param {0} of {1}", index, Tools.Readable(line));
        }
    }
}
