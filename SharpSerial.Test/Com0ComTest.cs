using System;
using NUnit.Framework;

namespace SharpSerial.Test
{
    public class Com0ComTest
    {
        private const string COM1 = "COM98";
        private const string COM2 = "COM99";

        [Test]
        public void DeviceDiscardCom0ComTest()
        {
            TestTools.DeviceDiscard(COM1, COM2);
        }

        [Test]
        public void DeviceBasicCom0ComTest()
        {
            TestTools.DeviceBasic(COM1, COM2);
        }

        [Test]
        public void ProcessDiscardCom0ComTest()
        {
            TestTools.ProcessDiscard(COM1, COM2);
        }

        [Test]
        public void ProcessBasicCom0ComTest()
        {
            TestTools.ProcessBasic(COM1, COM2);
        }
    }
}
