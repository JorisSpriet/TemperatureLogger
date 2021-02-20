using System;
using System.Runtime.InteropServices;
using System.Text;

namespace TemperatuurLogger.Protocol
{
	public static class Utils
	{
		public static T Map<T>(byte[] data)
		{
			GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
			var lfh = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
			handle.Free();
			return lfh;
		}

		public static T[] MapArray<T>(byte[] data)
		{
			//mapping bytes to arrays of structs is hell in C#...
			var objectSize = Marshal.SizeOf(typeof(T));
			var objectCount = data.Length / objectSize;
			T[] result = new T[objectCount];

			var chunk = new byte[objectSize];
			for (int i = 0; i < objectCount; i++) {
				Array.Copy(data, i * objectSize, chunk, 0, objectSize);
				result[i] = Map<T>(chunk);
			}
			return result;
		}
		public static int LengthOf<T>()
		{
			return Marshal.SizeOf(typeof(T));
		}

		public static void SetBcc(byte[] message)
		{
			byte s = 0;
			for (int i = 4; i < message.Length - 5; i++) s ^= message[i];
			message[message.Length - 5] = s;
		}

		internal static Array GetTimeBCD()
		{
			var now = DateTime.Now;
			var nowstr = now.ToString("ssmmHHddMMyyyy");
			var nowstrbytes = Encoding.ASCII.GetBytes(nowstr);
			var result = new byte[7];
			for (int i = 0; i < 14; i++) {
				int t = i / 2;
				byte s = (byte)(i % 2 == 0 ? 4 : 0);
				byte d = (byte)((nowstrbytes[i] - 0x30) << s);

				result[t] = (byte)(result[t] | d);
			}
			return result;
		}
	}
}
