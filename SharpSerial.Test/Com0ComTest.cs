using System;
using NUnit.Framework;

namespace SharpSerial.Test
{
    public class Com0ComTest
    {
        [Test]
        public void DeviceDiscardCom0ComTest()
        {
            TestTools.DeviceDiscard("COM98", "COM99");
        }

        [Test]
        public void DeviceBasicCom0ComTest()
        {
            TestTools.DeviceBasic("COM98", "COM99");
        }

        [Test]
        public void ProcessDiscardCom0ComTest()
        {
            TestTools.ProcessDiscard("COM98", "COM99");
        }

        [Test]
        public void ProcessBasicCom0ComTest()
        {
            TestTools.ProcessBasic("COM98", "COM99");
        }
    }
}
