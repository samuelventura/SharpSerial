using System;
using System.Threading;
using System.Collections.Generic;
using System.IO.Ports;

namespace SharpSerial
{
    // com0com BytesToRead is UNRELIABLE
    // Solution based on BaseStream and influenced by
    // https://www.sparxeng.com/blog/software/must-use-net-system-io-ports-serialport
    // https://www.vgies.com/a-reliable-serial-port-in-c/
    class SerialWrapper : ISerialInterface, IDisposable
    {
        private readonly SerialPort serial;
        private readonly List<byte> list;
        private readonly Queue<byte> queue;
        private readonly byte[] buffer;

        public SerialWrapper()
        {
            list = new List<byte>(256);
            queue = new Queue<byte>(256);
            buffer = new byte[256];
            serial = new SerialPort();
        }

        public void Dispose()
        {
            Tools.Try(serial.Close);
            Tools.Try(serial.Dispose);
        }

        public void SetProperty(string line)
        {
            var parts = line.Split(new char[] { '=' }, 2);
            var propertyName = parts[0];
            var propertyValue = parts[1];
            var property = serial.GetType().GetProperty(propertyName);
            var value = Convert.ChangeType(propertyValue, property.PropertyType);
            property.SetValue(serial, value, null);
        }

        public void Write(byte[] data)
        {
            InitAndOpenPort();
            serial.BaseStream.Write(data, 0, data.Length);
            //always flush to allow sync by following read available
            serial.BaseStream.Flush();
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
            if (!serial.IsOpen)
            {
                serial.Open();
                //DiscardInBuffer not needed by FTDI and ignored by com0com
                serial.BaseStream.BeginRead(buffer, 0, buffer.Length, ReadCallback, null);
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            int count = serial.BaseStream.EndRead(ar);
            lock (queue) for (var i = 0; i < count; i++) queue.Enqueue(buffer[i]);
            serial.BaseStream.BeginRead(buffer, 0, buffer.Length, ReadCallback, null);
        }
    }
}
