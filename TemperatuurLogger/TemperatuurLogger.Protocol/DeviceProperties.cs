using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TemperatuurLogger.Protocol
{
	public class DeviceProperties
	{
		public string SerialNumber { get; set; }

		public string Description { get; set; }

		public int NumberOfSamples { get; set; }
	}
}
