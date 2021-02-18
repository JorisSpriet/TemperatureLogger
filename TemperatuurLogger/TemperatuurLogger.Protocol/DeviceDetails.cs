namespace TemperatuurLogger.Protocol
{
	public class DeviceDetails
	{
		public string SerialNumber { get; set; }

		public string Description { get; set; }

		public string Model { get; set; }

		public int DelayTime { get; set; }

		public int SampleInterval { get; set; }

		public int NumberOfSamples { get; set; }

		public int LoggingInterval { get; set; }

		public decimal OffsetCh1 { get; set; }

		public decimal OffsetCh2 { get; set; }


	}

}
