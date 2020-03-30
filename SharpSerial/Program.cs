using System;

namespace SharpSerial
{
    partial class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += Tools.ExceptionHandler;

            using (var wrapper = new SerialDevice(Tools.ExceptionHandler))
            {
                foreach (var arg in args) Tools.SetProperty(wrapper.Serial, arg);
                var line = Console.ReadLine();
                while (line != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) break;
                    else if (line.StartsWith("$"))
                    {
                        if (line.Contains("=")) Tools.SetProperty(wrapper.Serial, line.Substring(1));
                        else
                        {
                            var parts = line.Split(new char[] { ',' });
                            switch (parts[0])
                            {
                                case "$r":
                                    if (parts.Length < 4) throw Tools.Make("Expected 4 parts for {0}", Tools.Readable(line));
                                    var rSize = ParseInt(line, parts[1], 1);
                                    var rEop = ParseInt(line, parts[2], 2);
                                    var rToms = ParseInt(line, parts[3], 3);
                                    var rData = wrapper.Read(rSize, rEop, rToms);
                                    Tools.AnswerHex(rData);
                                    break;
                                default:
                                    throw Tools.Make("Unknown command {0}", Tools.Readable(line));
                            }
                        }
                    }
                    else if (line.StartsWith(">"))
                    {
                        wrapper.Write(Tools.ParseHex(line));
                    }
                    else
                    {
                        throw Tools.Make("Unknown command {0}", Tools.Readable(line));
                    }
                    line = Console.ReadLine();
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
