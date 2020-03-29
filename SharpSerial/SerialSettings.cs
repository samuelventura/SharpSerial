using System;
using System.IO.Ports;
using System.Globalization;
using System.ComponentModel;

namespace SharpSerial
{
    public class SerialSettings
    {
        public SerialSettings() => CopyProperties(this, new SerialPort());

        public SerialSettings(SerialPort sp) => CopyProperties(this, sp);

        public void CopyFrom(SerialPort sp) => CopyProperties(this, sp);

        public void CopyTo(SerialPort sp) => CopyProperties(sp, this);

        public void CopyFrom(SerialSettings ss) => CopyProperties(this, ss);

        public void CopyTo(SerialSettings ss) => CopyProperties(ss, this);

        [TypeConverter(typeof(BaudRateOptions))]
        public int BaudRate { get; set; }

        public int DataBits { get; set; }

        public Parity Parity { get; set; }

        public Handshake Handshake { get; set; }

        public StopBits StopBits { get; set; }

        public bool DiscardNull { get; set; }

        public bool DtrEnable { get; set; }

        public int ReadTimeout { get; set; }

        public int ReadBufferSize { get; set; }

        public int WriteTimeout { get; set; }

        public int WriteBufferSize { get; set; }

        static void CopyProperties(Object source, Object target)
        {
            CopyProperty(source, target, "BaudRate");
            CopyProperty(source, target, "DataBits");
            CopyProperty(source, target, "Parity");
            CopyProperty(source, target, "Handshake");
            CopyProperty(source, target, "StopBits");
            CopyProperty(source, target, "DiscardNull");
            CopyProperty(source, target, "DtrEnable");
            CopyProperty(source, target, "ReadTimeout");
            CopyProperty(source, target, "ReadBufferSize");
            CopyProperty(source, target, "WriteTimeout");
            CopyProperty(source, target, "WriteBufferSize");
        }

        static void CopyProperty(Object source, Object target, string name)
        {
            var propertyTarget = target.GetType().GetProperty(name);
            var propertySource = source.GetType().GetProperty(name);
            propertyTarget.SetValue(target, propertySource.GetValue(source, null), null);
        }
    }

    public class BaudRateOptions : TypeConverter
    {
        public readonly static int[] BaudRates = new int[] {
            110,
            300,
            600,
            1200,
            2400,
            4800,
            9600,
            14400,
            19200,
            28800,
            38400,
            56000,
            57600,
            115200,
            128000,
            153600,
            230400,
            256000,
            460800,
            921600
        };

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(BaudRates);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
        {
            return int.Parse(value.ToString());
        }

        public override object ConvertTo(ITypeDescriptorContext context,
            CultureInfo culture, object value, Type destinationType)
        {
            return value.ToString();
        }
    }
}
