using System.IO.Ports;

namespace TemperatuurLogger.Protocol
{
	public class DeviceComPortDetails
	{
		public string PortName {  get; set; }

		public int BaudRate {  get; set;}

		public int DataBits { get; set; }

		public StopBits StopBits{ get; set; }

		public Parity Parity { get; set; }

		public static DeviceComPortDetails FromSerialPort(SerialPort serialPort) => 
			new DeviceComPortDetails {
				BaudRate = serialPort.BaudRate,
                DataBits = serialPort.DataBits,
                Parity = serialPort.Parity,
                PortName = serialPort.PortName,
				StopBits = serialPort.StopBits,
			};

		public SerialPort GetSerialPort()
		{
			var existing = SerialPortCache.Instance[this];
			if(existing==null) {
				existing = new SerialPort(PortName,BaudRate,Parity,DataBits,StopBits);
				existing.Open();
				SerialPortCache.Instance[this] = existing;
			}
			return existing;
		}
    }

}
