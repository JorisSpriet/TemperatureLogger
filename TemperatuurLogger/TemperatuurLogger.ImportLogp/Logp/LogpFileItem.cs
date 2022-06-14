using System;
using System.Text;
using System.Globalization;
using System.Runtime.InteropServices;

namespace TemperatuurLogger.ImportLogp
{

	/// <summary>
    /// Maps to binary structure of logp file produced by tool.
    /// </summary> 
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct LogpFileItem	
	{		
		[MarshalAs(UnmanagedType.ByValArray,SizeConst = 14)]
		public byte[]  DateTime; //yyyyMMddHHmmss
		
		public float Temperature; //IEE-754
				

		public DateTime TimeStamp()
		{
			var sDateTime = Encoding.ASCII.GetString(DateTime);
			return System.DateTime.ParseExact(sDateTime, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
		}
	}
	 
}
