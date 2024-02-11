using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TemperatuurLogger.Protocol
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct AnswerGetInfoDetails1Message
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] Header;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
		public byte[] SerialNumber;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[] AnswerCode;	

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] Description;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]

		public byte[] Unknown1;

		public byte Stx;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
		public byte[] Model;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] Unknown2;

		public ushort NumberOfSamples;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)] 
		public byte[] Unknown3;

		public short OffsetCh1;

		public short OffsetCh2;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] Unknown4;

		public short DelayTime;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public byte[] TailUnknown;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] Tail;

		public bool IsValid()
		{
			return Messages.Header.SequenceEqual(Header) &&
			       Messages.AnswerGetDataInfo1Command.SequenceEqual(AnswerCode) &&
			       Messages.Tail.SequenceEqual(Tail);
		}

		public string GetSerialNumber()
		{
			return Encoding.ASCII.GetString(SerialNumber);
		}

	}
}
