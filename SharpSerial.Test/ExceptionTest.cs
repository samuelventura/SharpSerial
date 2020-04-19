using System;
using System.IO;
using System.Diagnostics;
using NUnit.Framework;

namespace SharpSerial.Test
{
    public class ExceptionTest
    {
        [SetUp]
        public void Init()
        {
            //Stdio.EnableTrace(true, false);
            Stdio.Trace("-----------------------------------------");
            Stdio.Trace("{0}", TestContext.CurrentContext.Test.Name);
        }

        [Test]
        public void SerialExceptionOnReadTest()
        {
            var ss = new SerialSettings("COM0");
            using (var sp = new SerialProcess(ss))
            {
                Assert.Throws<SerialException>(() =>
                {
                    sp.Read(-1, -1, 200);
                });
            }
        }

        [Test]
        public void SerialExceptionOnWriteTest()
        {
            var ss = new SerialSettings("COM0");
            using (var sp = new SerialProcess(ss))
            {
                //stdout pipe remains open even after process exited
                //exception wont happend in stdout.writeline but
                //in detection of ! in beggining of stdin.readline
                Assert.Throws<SerialException>(() =>
                {
                    //write zero bytes is a good ping
                    sp.Write(new byte[0]);
                });
            }
        }

        [Test]
        public void EofExceptionOnReadTest()
        {
            var ss = new SerialSettings("COM98");
            using (var sp = new SerialProcess(ss))
            {
                Process.GetProcessById(sp.Pid).Kill();
                Assert.Throws<EndOfStreamException>(() =>
                {
                    sp.Read(-1, -1, 200);
                });
            }
        }

        [Test]
        public void EofExceptionOnWriteTest()
        {
            var ss = new SerialSettings("COM99");
            using (var sp = new SerialProcess(ss))
            {
                Process.GetProcessById(sp.Pid).Kill();
                Assert.Throws<EndOfStreamException>(() =>
                {
                    sp.Write(new byte[0]);
                });
            }
        }
    }
}
