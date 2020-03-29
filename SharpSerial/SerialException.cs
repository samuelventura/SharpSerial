using System;

namespace SharpSerial
{
    public class SerialException : Exception
    {
        private readonly string trace;

        public SerialException(string message, string trace) : base(message)
        {
            this.trace = trace;
        }

        public string Trace { get { return trace; } }
    }
}
