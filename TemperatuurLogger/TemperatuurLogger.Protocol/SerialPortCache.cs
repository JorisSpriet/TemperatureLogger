using System.Collections.Generic;
using System.IO.Ports;

namespace TemperatuurLogger.Protocol
{
    public class SerialPortCache
    {
        private static SerialPortCache instance = new SerialPortCache();

        private static Dictionary<string, SerialPort> ports = new Dictionary<string, SerialPort>();

        private SerialPortCache() { }

        public static SerialPortCache Instance { get { return instance; } }

        public SerialPort this[DeviceComPortDetails details]
        {
            get { return ports.TryGetValue(details.PortName, out var result) ? result : null; }
            set { ports[details.PortName] = value; }
        }

        public SerialPort this[string port]
        {
            get { return ports.TryGetValue(port, out var result) ? result : null; }
            set { ports[port] = value; }
        }


    }

}
