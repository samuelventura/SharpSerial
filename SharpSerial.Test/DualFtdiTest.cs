using System;
using NUnit.Framework;

namespace SharpSerial.Test
{
    public class DualFtdiTest
    {
        private const string COM1 = "COM10";
        private const string COM2 = "COM11";

        [Test]
        public void DeviceDiscardFtdiTest()
        {
            TestTools.DeviceDiscard(COM1, COM2);
        }

        [Test]
        public void DeviceBasicFtdiTest()
        {
            TestTools.DeviceBasic(COM1, COM2);
        }

        [Test]
        public void ProcessDiscardFtdiTest()
        {
            TestTools.ProcessDiscard(COM1, COM2);
        }

        [Test]
        public void ProcessBasicFtdiTest()
        {
            TestTools.ProcessBasic(COM1, COM2);
        }
    }
}
