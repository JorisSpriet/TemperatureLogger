using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace ATALoggerLib
{
	public class Logger : IDisposable, ILogger
	{
		private readonly SerialPort serialPort;

		public string SerialNumber { get; private set; }

		public LoggerDetails GetDetailsFromDevice()
		{
			serialPort.Write(Messages.GetDataInfo1Message(SerialNumber), 0, Messages.AnswerGetDataInfo1Command.Length);

			Thread.Sleep(500);
			var buffer = new byte[Marshal.SizeOf<AnswerGetInfoDetails1Message>()];
			var offset = 0;

			while (serialPort.BytesToRead > 0){
				var a = serialPort.BytesToRead;
				int r = serialPort.Read(buffer, offset, a);
				offset += r;
			}

			var details1= Utils.Map<AnswerGetInfoDetails1Message>(buffer);

			if (!details1.IsValid())
				throw new Exception("Received invalid details from device.");

			var result = new LoggerDetails
			{
				Description = Encoding.ASCII.GetString(details1.Description),
				SerialNumber = Encoding.ASCII.GetString(details1.SerialNumber),
				Info = Encoding.ASCII.GetString(details1.Info)
			};

			if(SerialNumber!=result.SerialNumber)
				throw new Exception("Received invalid details from device : different serial number");

			return result;
		}

		public void GetSamplesFromDevice(SamplesReadingCallback samplesReadingCallback)
		{


			var result = new List<LoggerSample>();



		}

		public void ClearDataOnDevice()
		{
			serialPort.Write(Messages.ClearDataMessage());
		}



		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.
				serialPort.Dispose();

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~Logger() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}

		
		#endregion

		public Logger(string serialNumber, SerialPort port)
		{
			SerialNumber = serialNumber;
			serialPort = port;
		}
	}

}
