using System;
using System.IO.Ports;
using NUnit.Framework;

namespace SharpSerial.Test
{
    public class SettingsTest
    {

        [Test]
        public void SetPropertySerialSettingsTest()
        {
            var ss = new SerialSettings();

            Assert.AreEqual(9600, ss.BaudRate);
            Tools.SetProperty(ss, "BaudRate=1200");
            Assert.AreEqual(1200, ss.BaudRate);

            Assert.AreEqual(8, ss.DataBits);
            Tools.SetProperty(ss, "DataBits=7");
            Assert.AreEqual(7, ss.DataBits);

            Assert.AreEqual(Parity.None, ss.Parity);
            Tools.SetProperty(ss, "Parity=Even");
            Assert.AreEqual(Parity.Even, ss.Parity);

            Assert.AreEqual(StopBits.One, ss.StopBits);
            Tools.SetProperty(ss, "StopBits=Two");
            Assert.AreEqual(StopBits.Two, ss.StopBits);

            Assert.AreEqual(Handshake.None, ss.Handshake);
            Tools.SetProperty(ss, "Handshake=XOnXOff");
            Assert.AreEqual(Handshake.XOnXOff, ss.Handshake);
        }

        [Test]
        public void SetPropertySerialPortTest()
        {
            var sp = new SerialPort();

            Assert.AreEqual(9600, sp.BaudRate);
            Tools.SetProperty(sp, "BaudRate=1200");
            Assert.AreEqual(1200, sp.BaudRate);

            Assert.AreEqual(8, sp.DataBits);
            Tools.SetProperty(sp, "DataBits=7");
            Assert.AreEqual(7, sp.DataBits);

            Assert.AreEqual(Parity.None, sp.Parity);
            Tools.SetProperty(sp, "Parity=Even");
            Assert.AreEqual(Parity.Even, sp.Parity);

            Assert.AreEqual(StopBits.One, sp.StopBits);
            Tools.SetProperty(sp, "StopBits=Two");
            Assert.AreEqual(StopBits.Two, sp.StopBits);

            Assert.AreEqual(Handshake.None, sp.Handshake);
            Tools.SetProperty(sp, "Handshake=XOnXOff");
            Assert.AreEqual(Handshake.XOnXOff, sp.Handshake);
        }

        [Test]
        public void SerialSettingsConstructorTest()
        {
            var sp1 = new SerialPort();
            Assert.AreEqual("COM1", sp1.PortName);
            Assert.AreEqual(9600, sp1.BaudRate);
            Assert.AreEqual(8, sp1.DataBits);
            Assert.AreEqual(Parity.None, sp1.Parity);
            Assert.AreEqual(StopBits.One, sp1.StopBits);
            Assert.AreEqual(Handshake.None, sp1.Handshake);

            var ss1 = new SerialSettings();
            Assert.AreEqual("COM1", ss1.PortName);
            Assert.AreEqual(9600, ss1.BaudRate);
            Assert.AreEqual(8, ss1.DataBits);
            Assert.AreEqual(Parity.None, ss1.Parity);
            Assert.AreEqual(StopBits.One, ss1.StopBits);
            Assert.AreEqual(Handshake.None, ss1.Handshake);

            var ss2 = new SerialSettings("COM0");
            Assert.AreEqual("COM0", ss2.PortName);
            Assert.AreEqual(9600, ss2.BaudRate);
            Assert.AreEqual(8, ss2.DataBits);
            Assert.AreEqual(Parity.None, ss2.Parity);
            Assert.AreEqual(StopBits.One, ss2.StopBits);
            Assert.AreEqual(Handshake.None, ss2.Handshake);

            var sp2 = new SerialPort();
            sp2.PortName = "COM0";
            sp2.BaudRate = 1200;
            sp2.DataBits = 7;
            sp2.Parity = Parity.Even;
            sp2.StopBits = StopBits.Two;
            sp2.Handshake = Handshake.XOnXOff;

            var ss3 = new SerialSettings(sp2);
            Assert.AreEqual("COM0", ss3.PortName);
            Assert.AreEqual(1200, ss3.BaudRate);
            Assert.AreEqual(7, ss3.DataBits);
            Assert.AreEqual(Parity.Even, ss3.Parity);
            Assert.AreEqual(StopBits.Two, ss3.StopBits);
            Assert.AreEqual(Handshake.XOnXOff, ss3.Handshake);

            var ss4 = new SerialSettings(ss3);
            Assert.AreEqual("COM0", ss4.PortName);
            Assert.AreEqual(1200, ss4.BaudRate);
            Assert.AreEqual(7, ss4.DataBits);
            Assert.AreEqual(Parity.Even, ss4.Parity);
            Assert.AreEqual(StopBits.Two, ss4.StopBits);
            Assert.AreEqual(Handshake.XOnXOff, ss4.Handshake);
        }

        [Test]
        public void SerialSettingsCopyTest()
        {
            var sp1 = new SerialPort();
            sp1.PortName = "COM0";
            sp1.BaudRate = 1200;
            sp1.DataBits = 7;
            sp1.Parity = Parity.Even;
            sp1.StopBits = StopBits.Two;
            sp1.Handshake = Handshake.XOnXOff;

            var ss1 = new SerialSettings();
            ss1.CopyFrom(sp1);
            Assert.AreEqual("COM0", ss1.PortName);
            Assert.AreEqual(1200, ss1.BaudRate);
            Assert.AreEqual(7, ss1.DataBits);
            Assert.AreEqual(Parity.Even, ss1.Parity);
            Assert.AreEqual(StopBits.Two, ss1.StopBits);
            Assert.AreEqual(Handshake.XOnXOff, ss1.Handshake);

            var sp2 = new SerialPort();
            ss1.CopyTo(sp2);
            Assert.AreEqual("COM0", sp2.PortName);
            Assert.AreEqual(1200, sp2.BaudRate);
            Assert.AreEqual(7, sp2.DataBits);
            Assert.AreEqual(Parity.Even, sp2.Parity);
            Assert.AreEqual(StopBits.Two, sp2.StopBits);
            Assert.AreEqual(Handshake.XOnXOff, sp2.Handshake);

            var ss3 = new SerialSettings();
            ss3.CopyFrom(ss1);
            Assert.AreEqual("COM0", ss3.PortName);
            Assert.AreEqual(1200, ss3.BaudRate);
            Assert.AreEqual(7, ss3.DataBits);
            Assert.AreEqual(Parity.Even, ss3.Parity);
            Assert.AreEqual(StopBits.Two, ss3.StopBits);
            Assert.AreEqual(Handshake.XOnXOff, ss3.Handshake);

            var ss4 = new SerialSettings();
            ss1.CopyTo(ss4);
            Assert.AreEqual("COM0", ss4.PortName);
            Assert.AreEqual(1200, ss4.BaudRate);
            Assert.AreEqual(7, ss4.DataBits);
            Assert.AreEqual(Parity.Even, ss4.Parity);
            Assert.AreEqual(StopBits.Two, ss4.StopBits);
            Assert.AreEqual(Handshake.XOnXOff, ss4.Handshake);
        }
    }
}
