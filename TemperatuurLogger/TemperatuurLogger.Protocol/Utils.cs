using System;
using System.Runtime.InteropServices;

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
            for (int i = 0; i < objectCount; i++)
            {
                Array.Copy(data, i * objectSize, chunk, 0, objectSize);
                result[i] = Map<T>(chunk);
            }
            return result;
        }
		public static int LengthOf<T>()
		{
			return Marshal.SizeOf(typeof(T));
		}

	}
}
