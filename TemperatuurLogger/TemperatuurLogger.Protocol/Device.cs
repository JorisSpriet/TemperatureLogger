using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace TemperatuurLogger.Protocol
{
	public class Device : IDisposable, IDevice
	{
        private readonly SerialPort serialPort;
        private readonly string port;
		private readonly bool x;

        public string SerialNumber { get; private set; }

		

		public DeviceDetails GetDetailsFromDevice()
		{
		    try
            {
                serialPort.Open();				
                serialPort.Write(Messages.GetDataInfo1Message(SerialNumber,x), 0, 20);

                Thread.Sleep(500);
                var buffer = new byte[Marshal.SizeOf<AnswerGetInfoDetails1Message>()];
                var offset = 0;

                while (serialPort.BytesToRead > 0)
                {
                    var a = serialPort.BytesToRead;
                    int r = serialPort.Read(buffer, offset, a);
                    offset += r;
                }
                Console.WriteLine($"Received {offset} bytes");

                var details1 = Utils.Map<AnswerGetInfoDetails1Message>(buffer);

                //TODO 0 JS REWORK IsValid 
                if (!details1.IsValid())
                    throw new Exception("Received invalid details from device.");

				serialPort.Write(Messages.GetDataInfo2Message(SerialNumber, x), 0, 20);
				Thread.Sleep(500);

				buffer = new byte[Marshal.SizeOf<AnswerGetInfoDetails2Message>()];
				offset = 0;

				while (serialPort.BytesToRead > 0) {
					var a = serialPort.BytesToRead;
					int r = serialPort.Read(buffer, offset, a);
					offset += r;
				}
				Console.WriteLine($"Received {offset} bytes");

				var details2 = Utils.Map<AnswerGetInfoDetails2Message>(buffer);

				var result = new DeviceDetails
				{
					Description = Encoding.ASCII.GetString(details1.Description, 0, 16),
					SerialNumber = Encoding.ASCII.GetString(details1.SerialNumber, 0, 10),
					Model = Encoding.ASCII.GetString(details1.Model, 0, 6),
					NumberOfSamples = details1.NumberOfSamples,

					SampleInterval = details2.SampleInterval,
					OffsetCh1 = Convert.ToDecimal(details1.OffsetCh1)/10M,
					OffsetCh2 = Convert.ToDecimal(details1.OffsetCh2)/10M,
					DelayTime =details1.DelayTime
					
                };

                if (SerialNumber != result.SerialNumber)
                    throw new Exception("Received invalid details from device : different serial number");



                return result;
            } finally {
                serialPort.Close();
            }
 
        }

		public void GetSamplesFromDevice(SamplesReadingCallback samplesReadingCallback)
		{
			//TODO 0 JS IMPLEMENT GetSamplesfromDevice

			var result = new List<DeviceSample>();



		}

		public void ClearDataOnDevice()
		{
			//TODO 0 Implemenet ClearDataOnDevice
			//serialPort.Write(Messages.ClearDataMessage());
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

		public Device(string serialNumber, SerialPort port)
		{
			SerialNumber = serialNumber;
			serialPort = port;
			x = serialNumber.StartsWith("HE");
		}
	}

}
