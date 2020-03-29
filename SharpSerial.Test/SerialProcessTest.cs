using System;
using System.Text;
using NUnit.Framework;

namespace SharpSerial.Test
{
    public class SerialProcessTest
    {
        [Test]
        public void DiscardCom0ComTest()
        {
            Discard(p(98), p(99), "com0com");
        }

        [Test]
        public void DiscardFtdiTest()
        {
            Discard(p(10), p(11), "ftdi");
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

        public void Discard(string pn1, string pn2, string type)
        {
            var toms = 200;
            //using (var sp2 = new SerialProcess(pn2))
            using (var sp1 = new SerialProcess(pn1))
            {
                //sp2.Read(-1, -1, toms); //discard
                sp1.Write(b("Hello\n"));
                //read available to ensure flushed
                sp1.Read(-1, -1, 0);
            }
            using (var sp2 = new SerialProcess(pn2))
            {
                switch (type)
                {
                    case "ftdi":
                        //DiscardInBuffer auto applied on open
                        Assert.AreEqual("", s(sp2.Read(-1, -1, toms)));
                        break;
                    case "com0com":
                        //DiscardInBuffer not applied on open
                        Assert.AreEqual("Hello\n", s(sp2.Read(-1, -1, toms)));
                        break;
                }
            }
        }

        public void Basic(string pn1, string pn2)
        {
            var toms = 200;
            using (var sp1 = new SerialProcess(pn1))
            using (var sp2 = new SerialProcess(pn2))
            {
                sp2.Read(-1, -1, toms); //discard
                sp1.Write(b("Hello\n"));
                Assert.AreEqual("Hello\n", s(sp2.Read(-1, '\n', toms)));
                sp1.Write(b("Hello\n"));
                Assert.AreEqual("Hello\n", s(sp2.Read(6, -1, toms)));
                for (var i = 0; i < 100; i++)
                {
                    sp1.Write(b("Hello\n"));
                    Assert.AreEqual("H", s(sp2.Read(1, -1, toms)));
                    Assert.AreEqual("e", s(sp2.Read(-1, 'e', toms)));
                    Assert.AreEqual("ll", s(sp2.Read(2, '\n', toms)));
                    Assert.AreEqual("o\n", s(sp2.Read(3, '\n', toms)));
                }
                Assert.AreEqual("", s(sp2.Read(-1, -1, toms)));
            }
        }

        byte[] b(string s) => Encoding.ASCII.GetBytes(s);
        string s(byte[] b) => Encoding.ASCII.GetString(b);
        string p(int n) => string.Format("COM{0}", n);
    }
}
