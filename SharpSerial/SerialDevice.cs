using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.IO.Ports;

namespace SharpSerial
{
    // com0com BytesToRead is UNRELIABLE
    // Solution based on BaseStream and influenced by
    // https://www.sparxeng.com/blog/software/must-use-net-system-io-ports-serialport
    // https://www.vgies.com/a-reliable-serial-port-in-c/
    public class SerialDevice : ISerialStream, IDisposable
    {
        private readonly Action<Exception> handler;
        private readonly SerialPort serial;
        private readonly List<byte> list;
        private readonly Queue<byte> queue;
        private byte[] buffer;

        public SerialDevice(object settings, Action<Exception> handler = null)
        {
            this.handler = handler;
            this.list = new List<byte>(256);
            this.queue = new Queue<byte>(256);
            this.serial = new SerialPort();
            SerialSettings.CopyProperties(settings, serial);
        }

        public void Dispose()
        {
            Tools.Try(serial.Close);
            Tools.Try(serial.Dispose);
        }

        public void Write(byte[] data)
        {
            InitAndOpenPort();
            var stream = serial.BaseStream;
            stream.Write(data, 0, data.Length);
            //always flush to allow sync by following read available
            stream.Flush();
        }

        public byte[] Read(int size, int eop, int toms)
        {
            InitAndOpenPort();
            list.Clear();
            var dl = DateTime.Now.AddMilliseconds(toms);
            while (true)
            {
                int b = ReadByte();
                if (b == -1)
                {
                    //toms=0 should return immediately with available
                    if (DateTime.Now >= dl) break;
                    Thread.Sleep(1);
                    continue;
                }
                list.Add((byte)b);
                if (eop >= 0 && b == eop) break;
                if (size >= 0 && list.Count >= size) break;
                dl = DateTime.Now.AddMilliseconds(toms);
            }
            return list.ToArray();
        }

        private int ReadByte()
        {
            lock (queue)
            {
                if (queue.Count == 0) return -1;
                return queue.Dequeue();
            }
        }

        private void InitAndOpenPort()
        {
            if (buffer == null) //init flag
            {
                Tools.Assert(!serial.IsOpen, "Port {0} already open", serial.PortName);
                serial.Open(); //must not be already open
                buffer = new byte[256];
                //DiscardInBuffer not needed by FTDI and ignored by com0com
                var stream = serial.BaseStream;
                //unavailable after closed so pass it
                stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, stream);
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            Tools.Try(() =>
            {
                //try needed to avoid triggering the domain unhandled 
                //exception handler when used as standalone stream
                var stream = ar.AsyncState as Stream;
                int count = stream.EndRead(ar);
                if (count > 0) //0 for closed stream
                {
                    lock (queue) for (var i = 0; i < count; i++) queue.Enqueue(buffer[i]);
                    stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, stream);
                }
            }, handler);
        }
    }
}
