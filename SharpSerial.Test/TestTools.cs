using System;
using System.Text;
using System.IO.Ports;
using System.Threading;
using NUnit.Framework;
using SharpModbus.Serial;

namespace SharpSerial.Test
{
    public static class TestTools
    {
        public static byte[] b(string s) => Encoding.ASCII.GetBytes(s);
        public static string s(byte[] b) => Encoding.ASCII.GetString(b);

        public static void DeviceBasic(string pn1, string pn2)
        {
            foreach (var ss in SS()) Basic(pn1, pn2, (pn) =>
            {
                ss.PortName = pn;
                return new SerialDevice(ss);
            });
        }

        public static void ProcessBasic(string pn1, string pn2)
        {
            foreach (var ss in SS()) Basic(pn1, pn2, (pn) =>
            {
                ss.PortName = pn;
                return new SerialProcess(ss);
            });
        }

        public static void DeviceDiscard(string pn1, string pn2)
        {
            foreach (var ss in SS()) Discard(pn1, pn2, (pn) =>
            {
                ss.PortName = pn;
                return new SerialDevice(ss);
            });
        }

        public static void ProcessDiscard(string pn1, string pn2)
        {
            foreach (var ss in SS()) Discard(pn1, pn2, (pn) =>
            {
                ss.PortName = pn;
                return new SerialProcess(ss);
            });
        }

        public static SerialSettings[] SS()
        {
            return new SerialSettings[] {
                new SerialSettings(),
                new SerialSettings {
                    BaudRate = 115200,
                    DataBits = 7,
                    Parity = Parity.Even,
                    StopBits = StopBits.Two,
                    Handshake = Handshake.XOnXOff,
                },
            };
        }

        //issue: on fast close/open com0com wont clear inbuf
        //issue: on fast close/open ftdi wont release port
        public static void Discard(string pn1, string pn2, Func<string, ISerialStream> factory)
        {
            //ftdi requires delay to ensure port is released
            Thread.Sleep(400);
            using (var sp2 = factory(pn2))
            using (var sp1 = factory(pn1))
            {
                sp2.Read(-1, -1, 200); //discard
                sp1.Write(b("Hello\n"));
                //read available to ensure flushed
                sp1.Read(-1, -1, 0);
            }
            //ftdi requires delay to ensure port is released
            Thread.Sleep(400);
            using (var sp2 = factory(pn2))
            {
                //locks like delay above clears the com0com inbuf 
                Assert.AreEqual("", s(sp2.Read(-1, -1, 200)));
            }
        }

        public static void Basic(string pn1, string pn2, Func<string, ISerialStream> factory)
        {
            //ftdi requires delay to ensure port is released
            Thread.Sleep(400);
            using (var sp1 = factory(pn1))
            using (var sp2 = factory(pn2))
            {
                sp2.Read(-1, -1, 200); //discard
                sp1.Write(b("Hello\n"));
                Assert.AreEqual("Hello\n", s(sp2.Read(-1, '\n', 200)));
                sp1.Write(b("Hello\n"));
                Assert.AreEqual("Hello\n", s(sp2.Read(6, -1, 200)));
                for (var i = 0; i < 100; i++)
                {
                    sp1.Write(b("Hello\n"));
                    Assert.AreEqual("H", s(sp2.Read(1, -1, 200)));
                    Assert.AreEqual("e", s(sp2.Read(-1, 'e', 200)));
                    Assert.AreEqual("ll", s(sp2.Read(2, '\n', 200)));
                    Assert.AreEqual("o\n", s(sp2.Read(3, '\n', 200)));
                }
                Assert.AreEqual("", s(sp2.Read(-1, -1, 200)));
            }
        }
    }
}
