using System;
using System.Threading;
using System.Collections.Generic;
using System.IO.Ports;

namespace SharpSerial
{
    class SerialWrapper : ISerialInterface
    {
        private readonly SerialPort serial;
        private readonly List<byte> list;

        public SerialWrapper()
        {
            list = new List<byte>(256);
            serial = new SerialPort();
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
            if (!serial.IsOpen) serial.Open();
            serial.Write(data, 0, data.Length);
        }

        public byte[] Read(int size, int eop, int toms)
        {
            if (!serial.IsOpen) serial.Open();
            list.Clear();
            var dl = DateTime.Now.AddMilliseconds(toms);
            while (true)
            {
                if (serial.BytesToRead == 0)
                {
                    //toms=0 should return immediately with all available
                    if (DateTime.Now >= dl) break;
                    Thread.Sleep(1);
                    continue;
                }
                var b = (byte)serial.ReadByte();
                list.Add(b);
                if (eop >= 0 && b == eop) break;
                if (size >= 0 && list.Count >= size) break;
                dl = DateTime.Now.AddMilliseconds(toms);
            }
            return list.ToArray();
        }
    }
}
