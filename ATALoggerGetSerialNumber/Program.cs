using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATALogger;

namespace ATALoggerGetSerialNumber
{
	class Program
	{
		static void Main(string[] args)
		{
			var lf = new LoggerFinder();
			var logger = lf.FindLoggerPort("COM3");

			var ld = logger?.GetDetailsFromDevice();

			Console.WriteLine($"Serialnumber : {ld?.SerialNumber}");
			Console.WriteLine($"Description  : {ld?.Description}");
			Console.WriteLine($"Info         : {ld?.Info}");

			logger?.Dispose();

			Console.ReadLine();
		}
	}
}
