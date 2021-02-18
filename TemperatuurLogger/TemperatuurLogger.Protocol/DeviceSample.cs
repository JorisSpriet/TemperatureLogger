using System;

namespace TemperatuurLogger.Protocol
{
	public class DeviceSample
	{
		public long ID { get; set; }

		public DateTime TimeStamp { get; set; }

		public decimal Temperature { get; set; }
	}

}
