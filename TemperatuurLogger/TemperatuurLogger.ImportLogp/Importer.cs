using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace TemperatuurLogger.ImportLogp
{
	/// <summary>
    /// Processes logp file for storage into database.
    /// </summary>
	internal class Importer
	{
		internal LogpFile Import(string fileName)
		{
			var items = new List<LogpFileItem>();
            LogpFileHeader lfh;

            using (var fs = File.OpenRead(fileName)) {
				var br = new BinaryReader(fs);

				var readBuffer = br.ReadBytes(LengthOf<LogpFileHeader>());

                lfh = Map<LogpFileHeader>(readBuffer);
                WriteLine($"Logger Name:	    {lfh.LoggerName}");
				WriteLine($"Serial Number:	    {lfh.SerialNumber}");
				WriteLine($"Logs Amount:	    {lfh.LogsAmount}");
				WriteLine($"SN	DATE	TIME	{lfh.Unit}");
				//Console.WriteLine(lfh.Unit);
				for (int i = 0; i < lfh.LogsAmount; i++) {
					readBuffer = br.ReadBytes(LengthOf<LogpFileItem>());
					var li = Map<LogpFileItem>(readBuffer);
                    items.Add(li);
                    WriteLine($"{i+1:d4}\t{li.TimeStamp().ToString("yyyy-MM-dd HH:mm:ss")}\t{li.Temperature}");
				}
			}
            return new LogpFile
            {
                Header = lfh,
                Items = items.ToArray()
            };
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
