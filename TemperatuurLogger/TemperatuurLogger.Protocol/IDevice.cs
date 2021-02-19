using System.Collections.Generic;
using System.Threading.Tasks;

namespace TemperatuurLogger.Protocol
{
	public interface  IDevice
	{
		string SerialNumber { get; }

		DeviceDetails GetDetailsFromDevice();

		Task<DeviceSample[]> GetSamplesFromDevice(SamplesReadingCallback samplesReadingCallback);

		void ClearDataOnDevice();

		void Dispose();
	}

	public delegate void SamplesReadingCallback(DeviceSample[] readSamples, int totalNumberOfSamples, int offSet);

}
