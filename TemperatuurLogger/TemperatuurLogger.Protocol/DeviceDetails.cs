using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace TemperatuurLogger.Protocol
{
	public class DeviceDetails
	{
		public string SerialNumber { get; set; }

		public string Description { get; set; }

		public string Info { get; set; }
	}

}
