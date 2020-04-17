using System;
using System.Text;
using System.IO.Ports;
using System.Threading;
using NUnit.Framework;

namespace SharpSerial.Test
{
    public class SerialProcessTest
    {
        private const int TOMS = 200;

        [Test]
        public void SerialExceptionTest()
        {
            Assert.Throws<SerialException>(() =>
            {
                var ss = new SerialSettings();
                ss.PortName = p(9999);
                using (var sp = new SerialProcess(ss))
                {
                    sp.Read(-1, -1, TOMS);
                }
            });
        }

        [Test]
        public void DiscardCom0ComTest()
        {
            foreach (var ss in SS()) Discard(p(98), p(99), ss);
        }

        [Test]
        public void DiscardFtdiTest()
        {
            foreach (var ss in SS()) Discard(p(10), p(11), ss);
        }

        [Test]
        public void BasicFtdiTest()
        {
            foreach (var ss in SS()) Basic(p(10), p(11), ss);
        }

        [Test]
        public void BasicCom0ComTest()
        {
            foreach (var ss in SS()) Basic(p(98), p(99), ss);
        }

        public SerialSettings[] SS()
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
        public void Discard(string pn1, string pn2, SerialSettings ss)
        {
            //ftdi requires delay to ensure port is released
            Thread.Sleep(TOMS);
            var ss1 = new SerialSettings(ss);
            var ss2 = new SerialSettings(ss);
            ss1.PortName = pn1;
            ss2.PortName = pn2;
            using (var sp2 = new SerialProcess(ss2))
            using (var sp1 = new SerialProcess(ss1))
            {
                sp2.Read(-1, -1, TOMS); //discard
                sp1.Write(b("Hello\n"));
                //read available to ensure flushed
                sp1.Read(-1, -1, 0);
            }
            //ftdi requires delay to ensure port is released
            Thread.Sleep(TOMS);
            using (var sp2 = new SerialProcess(ss2))
            {
                //locks like delay above clears the com0com inbuf 
                Assert.AreEqual("", s(sp2.Read(-1, -1, TOMS)));
            }
        }

        public void Basic(string pn1, string pn2, SerialSettings ss)
        {
            //ftdi requires delay to ensure port is released
            Thread.Sleep(TOMS);
            var ss1 = new SerialSettings(ss);
            var ss2 = new SerialSettings(ss);
            ss1.PortName = pn1;
            ss2.PortName = pn2;
            using (var sp1 = new SerialProcess(ss1))
            using (var sp2 = new SerialProcess(ss2))
            {
                sp2.Read(-1, -1, TOMS); //discard
                sp1.Write(b("Hello\n"));
                Assert.AreEqual("Hello\n", s(sp2.Read(-1, '\n', TOMS)));
                sp1.Write(b("Hello\n"));
                Assert.AreEqual("Hello\n", s(sp2.Read(6, -1, TOMS)));
                for (var i = 0; i < 100; i++)
                {
                    sp1.Write(b("Hello\n"));
                    Assert.AreEqual("H", s(sp2.Read(1, -1, TOMS)));
                    Assert.AreEqual("e", s(sp2.Read(-1, 'e', TOMS)));
                    Assert.AreEqual("ll", s(sp2.Read(2, '\n', TOMS)));
                    Assert.AreEqual("o\n", s(sp2.Read(3, '\n', TOMS)));
                }
                Assert.AreEqual("", s(sp2.Read(-1, -1, TOMS)));
            }
        }

        byte[] b(string s) => Encoding.ASCII.GetBytes(s);
        string s(byte[] b) => Encoding.ASCII.GetString(b);
        string p(int n) => string.Format("COM{0}", n);
    }
}
