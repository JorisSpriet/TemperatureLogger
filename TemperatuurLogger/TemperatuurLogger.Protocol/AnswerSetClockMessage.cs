using System.Runtime.InteropServices;

namespace TemperatuurLogger.Protocol
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
	public struct AnswerSetClockMessage
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] Header;
		//actually it seems to return 10 first characers of name
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
		public byte[] SerialNumber;

		//expect : 0x5c 0x01
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[] AnswerCode;

		public byte BCC;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] Tail;

	}
}
