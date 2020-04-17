using System;

namespace SharpSerial
{
    partial class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += Tools.ExceptionHandler;

            var settings = new SerialSettings();
            foreach (var arg in args) Tools.SetProperty(settings, arg);
            using (var serial = new SerialDevice(settings))
            {
                var line = Stdio.ReadLine();
                while (!string.IsNullOrWhiteSpace(line))
                {
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
                                Tools.AnswerHex(rData);
                                break;
                            default:
                                throw Tools.Make("Unknown command {0}", Tools.Readable(line));
                        }
                    }
                    else if (line.StartsWith(">"))
                    {
                        serial.Write(Tools.ParseHex(line));
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
