using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ATALogger
{
	public class FileLogReader
	{
		public static FileLog Read(string fileName)
		{
			using (var fs = File.OpenRead(fileName))
			{
				return Read(fs);
			}
		}

		public static FileLog Read(Stream s)
		{
			var header = ReadHeader(s);
			var itemcount = header.LogItemCount;

			var items = ReadItems(s, itemcount);

			return new FileLog(header, items);
		}

		private static FileLogHeader ReadHeader(Stream fs)
		{
			var l = Marshal.SizeOf(typeof(FileLogHeader));
			byte[] buffer = new byte[l];
			var r = fs.Read(buffer, 0, buffer.Length);
			if (r != l)
			{
				throw new Exception("Did not read complete structure");
			}
			GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			var result = (FileLogHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(FileLogHeader));
			handle.Free();
			return result;
		}

		private static FileLogItem[] ReadItems(Stream fs, int count)
		{
			var result = new FileLogItem[count];
			var l = Marshal.SizeOf(typeof(FileLogItem));
			byte[] buffer = new byte[l];
			for (int i = 0; i < count; i++)
			{
				var r = fs.Read(buffer, 0, buffer.Length);
				if (r != buffer.Length)
				{
					throw new Exception("Did not read complete structure");
				}
				GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
				result[i] = (FileLogItem)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(FileLogItem));
				handle.Free();
			}

			return result;
		}
	}
}
