
using SharpModbus;
using SharpModbus.Serial;

namespace SharpSerial.Modbus {
    public abstract class ModbusMaster {
        
        public static SharpModbus.ModbusMaster IsolatedRTU(SerialSettings settings, int timeout = 400)
        {
            var stream = new ModbusIsolatedStream(settings, timeout);
            var protocol = new ModbusRTUProtocol();
            return new SharpModbus.ModbusMaster(stream, protocol);
        }
    }
}