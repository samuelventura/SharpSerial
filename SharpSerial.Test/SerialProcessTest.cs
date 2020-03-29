using System;
using System.Text;
using NUnit.Framework;

namespace SharpSerial.Test
{
    public class SerialProcessTest
    {
        [Test]
        public void BasicTest()
        {
            var sp1 = new SerialProcess("COM98");
            var sp2 = new SerialProcess("COM99");
            sp1.Write(b("Hello\n"));
            Assert.AreEqual("Hello\n", s(sp2.Read(-1, '\n', 200)));
            sp1.Write(b("Hello\n"));
            Assert.AreEqual("Hello", s(sp2.Read(5, '\n', 200)));
            Assert.AreEqual("\n", s(sp2.Read(-1, '\n', 200)));
            sp1.Write(b("Hello\n"));
            Assert.AreEqual("Hello\n", s(sp2.Read(-1, -1, 200)));
        }

        byte[] b(string s) => Encoding.ASCII.GetBytes(s);
        string s(byte[] b) => Encoding.ASCII.GetString(b);
    }
}
