using System.Runtime.InteropServices;

namespace TemperatuurLogger.ImportLogp
{
	/// <summary>
    /// Maps to binary structure of logp file produced by tool.
    /// </summary>    
	[StructLayout(LayoutKind.Sequential,Pack=1)]
	public struct LogpFileHeader
	{
		#pragma warning disable 1591
		public const int Length = 60;

		[MarshalAs(UnmanagedType.ByValTStr,SizeConst = 17)]
		public string /*char[29]*/ LoggerName;

		public byte b01;
		public byte b02;
		public byte b03;
		public byte b04;
		public byte b05;
		public byte b06;
		public byte b07;
		public byte b08;
		public byte b09;
		public byte b10;
		public byte b11;
		public byte b12;
		//

		//[FieldOffset(29)] 
		public int LogsAmount;

		//[FieldOffset(33)] 
		public int Reserved1; //0x00000001
		
		//[FieldOffset(37)] 
		public int Reserved2; //0x00000002

		//[FieldOffset(41)]
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
		public string /*char[10]*/ SerialNumber;

		//[FieldOffset(52)]
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 7)]
		public string /*char[7]*/ Unit;
		
		//[FieldOffset(58)] 
		public int Reserved3; //0x00000001	

		public byte b13;
	}

}
