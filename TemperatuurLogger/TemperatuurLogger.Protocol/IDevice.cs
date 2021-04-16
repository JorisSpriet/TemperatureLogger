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

	public delegate void SamplesReadingCallback(int percentageCompleted, int samplesRead, int totalNumberOrSamples);

}
