using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace ATALoggerLib
{
	public class LoggerFinder
	{
		public Logger FindLoggerPort(string preferredPort)
		{
			var currentPortNames = SerialPort.GetPortNames();
			if (currentPortNames.Contains(preferredPort)){
				var logger = LoggerFoundOnPort(preferredPort);
				if (logger != null)
					return logger;
			} 

			foreach (var portName in currentPortNames){
				var logger = LoggerFoundOnPort(portName);
				if (logger != null)
					return logger;
			}

			return null;
		}

		private Logger LoggerFoundOnPort(string port)
		{
			var serialPort = new SerialPort(port,38400);
			serialPort.Open();

			serialPort.Write(Messages.GetSerialNumberMessage(), 0, 20);
			Thread.Sleep(500);
			var answer = new byte[20];
			var offset = 0;

			while (serialPort.BytesToRead > 0 && offset<20) {				
				int r = serialPort.Read(answer,offset, serialPort.BytesToRead);
				offset += r;
				if (r == 0)
					break;
			}

			if (offset == Utils.LengthOf<AnswerGetSerialNumberMessage>()){
				//expect
				// header
				// serial number (10 bytes)
				// answer code (2 bytes)
				// tail (4 bytes)
				try{
					var a = Utils.Map<AnswerGetSerialNumberMessage>(answer);
					if (a.IsValid()){
						return new Logger(a.GetSerialNumber(), serialPort);
					}
				} catch{
					serialPort.Close();
				}

			}

			return null;

		}

	}

}
