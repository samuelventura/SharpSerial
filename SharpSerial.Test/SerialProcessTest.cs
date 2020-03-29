using System;
using System.Text;
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
                using (var sp = new SerialProcess(p(9999)))
                {
                    sp.Read(-1, -1, TOMS);
                }
            });
        }

        [Test]
        public void DiscardCom0ComTest()
        {
            Discard(p(98), p(99));
        }

        [Test]
        public void DiscardFtdiTest()
        {
            Discard(p(10), p(11));
        }

        [Test]
        public void BasicFtdiTest()
        {
            Basic(p(10), p(11));
        }

        [Test]
        public void BasicCom0ComTest()
        {
            Basic(p(98), p(99));
        }

        //issue: on fast close/open com0com wont clear inbuf
        //issue: on fast close/open ftdi wont release port
        public void Discard(string pn1, string pn2)
        {
            using (var sp2 = new SerialProcess(pn2))
            using (var sp1 = new SerialProcess(pn1))
            {
                sp2.Read(-1, -1, TOMS); //discard
                sp1.Write(b("Hello\n"));
                //read available to ensure flushed
                sp1.Read(-1, -1, 0);
            }
            //ftdi requires delay to ensure port is released
            Thread.Sleep(TOMS);
            using (var sp2 = new SerialProcess(pn2))
            {
                //locks like delay above clears the com0com inbuf 
                Assert.AreEqual("", s(sp2.Read(-1, -1, TOMS)));
            }
        }

        public void Basic(string pn1, string pn2)
        {
            using (var sp1 = new SerialProcess(pn1))
            using (var sp2 = new SerialProcess(pn2))
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
