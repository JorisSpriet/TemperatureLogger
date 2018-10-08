using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReadLogpFile
{
	public class TemperatureLog
	{
		public string LoggerName { get; set; }

		public DateTime Date { get; set; }

		public DateTime Time { get; set; }

		public float Temperature { get; set; }
	}
}
