using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TemperatuurLogger.ImportLogp
{
	public class Importer
	{
		public IEnumerable<TemperatureLog> Import(string fileName)
		{
			var result = new List<TemperatureLog>();

			using (var fs = File.OpenRead("data.logp")) {
				var br = new BinaryReader(fs);

				var readBuffer = br.ReadBytes(LengthOf<LogFileHeader>());

				var lfh = Map<LogFileHeader>(readBuffer)
				WriteLine($"Logger Name:	    {lfh.LoggerName}");
				WriteLine($"Serial Number:	    {lfh.SerialNumber}");
				WriteLine($"Logs Amount:	    {lfh.LogsAmount}");
				WriteLine($"SN	DATE	TIME	{lfh.Unit}");
				//Console.WriteLine(lfh.Unit);
				for (int i = 0; i < lfh.LogsAmount; i++) {
					readBuffer = br.ReadBytes(LengthOf<LogItem>());
					var li = Map<LogItem>(readBuffer);
					Console.WriteLine("{0}\t{1}\t{2}\t{3}", i + 1, li.TimeStamp().ToString("yyyy-MM-dd"),
						li.TimeStamp().ToString("HH:mm:ss"), li.Temperature);
				}

			}
		}

		private bool Verbose { get; set;}

		private void WriteLine(string line, bool verbose = false) {
			if(verbose && !Verbose)
				return;
			Console.WriteLine(line);
		}

		private static T Map<T>(byte[] data)
		{
			GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
			var lfh = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
			handle.Free();
			return lfh;
		}

		private static int LengthOf<T>()
		{
			return Marshal.SizeOf(typeof(T));
		}
	}
}
