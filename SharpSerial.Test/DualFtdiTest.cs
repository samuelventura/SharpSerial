using System;
using NUnit.Framework;

namespace SharpSerial.Test
{
    public class DualFtdiTest
    {
        [Test]
        public void DeviceDiscardFtdiTest()
        {
            TestTools.DeviceDiscard("COM10", "COM11");
        }

        [Test]
        public void DeviceBasicFtdiTest()
        {
            TestTools.DeviceBasic("COM10", "COM11");
        }

        [Test]
        public void ProcessDiscardFtdiTest()
        {
            TestTools.ProcessDiscard("COM10", "COM11");
        }

        [Test]
        public void ProcessBasicFtdiTest()
        {
            TestTools.ProcessBasic("COM10", "COM11");
        }
    }
}
