using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ReadLogpFile
{

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct LogItem
	{
		//[FieldOffset(0)] 
		[MarshalAs(UnmanagedType.ByValArray,SizeConst = 14)]
		public byte[] /*char[]*/ DateTime; //yyyyMMddHHmmss
		//[FieldOffset(14)] 
		public float Temperature; //IEE-754
		//[FieldOffset(18)] 
		//public byte Reserved; //@

		public DateTime TimeStamp()
		{
			var sDateTime = Encoding.ASCII.GetString(DateTime);
			return System.DateTime.ParseExact(sDateTime, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
		}
	}
	 
}
