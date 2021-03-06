﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ReadLogpFile
{
	public class Importer
	{
		public IEnumerable<TemperatureLog> Import(string fileName)
		{
			var result = new List<TemperatureLog>();

			using (var fs = File.OpenRead("data.logp")) {
				var br = new BinaryReader(fs);

				var readBuffer = br.ReadBytes(LengthOf<LogFileHeader>());

				var lfh = Map<LogFileHeader>(readBuffer);

				Console.WriteLine("Logger Name:	" + lfh.LoggerName);
				Console.WriteLine("Serial Number:	" + lfh.SerialNumber);
				Console.WriteLine("Logs Amount:	" + lfh.LogsAmount);
				Console.WriteLine("SN	DATE	TIME	" + lfh.Unit);
				//Console.WriteLine(lfh.Unit);
				for (int i = 0; i < lfh.LogsAmount; i++) {
					readBuffer = br.ReadBytes(LengthOf<LogItem>());
					var li = Map<LogItem>(readBuffer);
					Console.WriteLine("{0}\t{1}\t{2}\t{3}", i + 1, li.TimeStamp().ToString("yyyy-MM-dd"),
						li.TimeStamp().ToString("HH:mm:ss"), li.Temperature);
				}

			}
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
