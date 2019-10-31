using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using log4net;

namespace ATALogger
{
	public class LoggerFinder
	{
		public static ILog logger = LogManager.GetLogger(typeof(LoggerFinder));

		public Logger FindLoggerPort(string preferredPort)
		{
			var currentPortNames = SerialPort.GetPortNames();
			if (currentPortNames.Contains(preferredPort))
			{
				var logger = LoggerFoundOnPort(preferredPort);
				if (logger != null)
					return logger;
			}

			foreach (var portName in currentPortNames)
			{
				var logger = LoggerFoundOnPort(portName);
				if (logger != null)
					return logger;
			}

			return null;
		}

		private Logger LoggerFoundOnPort(string port)
		{

			var serialPort = new SerialPort(port, 38400,Parity.None,8,StopBits.One);
			try
			{
				serialPort.Open();
				logger.Info($"Succesfully opened serial port {port}");

				serialPort.Write(Messages.GetSerialNumberMessage(), 0, 20);
				Thread.Sleep(500);
				var buffer = new byte[1024];
				var expectedAnswerLength = Utils.LengthOf<AnswerGetSerialNumberMessage>();
				var answer = new byte[expectedAnswerLength];
				var totalReceived = 0;

				while (serialPort.BytesToRead > 0 && totalReceived < expectedAnswerLength)
				{
					//var readSize = serialPort.BytesToRead
					int r = serialPort.Read(buffer, totalReceived, serialPort.BytesToRead);
					totalReceived += r;
					if (r == 0)
						break;
				}
				Array.Copy(buffer,0,answer,0,expectedAnswerLength);

				if (totalReceived > expectedAnswerLength)
				{
					logger.Warn($"Received {totalReceived} > {expectedAnswerLength} while retrieving SerialNumber from logger on {port}");
				}

				if (totalReceived >= expectedAnswerLength  )
				{
					//expect
					// header
					// serial number (10 bytes)
					// answer code (2 bytes)
					// tail (4 bytes)
					try
					{
						var a = Utils.Map<AnswerGetSerialNumberMessage>(answer);
						if (a.IsValid())
						{
							return new Logger(a.GetSerialNumber(), serialPort);
						}
					}
					catch
					{
					}

				}

				return null;
			}
			finally
			{
				serialPort.Close();
			}

		}

	}

}