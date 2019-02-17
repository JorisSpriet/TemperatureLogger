using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace ATALogger
{
	public interface  ILogger
	{
		string SerialNumber { get; }

		LoggerDetails GetDetailsFromDevice();

		void GetSamplesFromDevice(SamplesReadingCallback samplesReadingCallback);

		void ClearDataOnDevice();

		void Dispose();
	}

	public delegate void SamplesReadingCallback(LoggerSample[] readSamples, int totalNumberOfSamples, int offSet);

}
