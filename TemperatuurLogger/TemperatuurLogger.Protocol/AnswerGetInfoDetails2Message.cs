﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TemperatuurLogger.Protocol
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct AnswerGetInfoDetails2Message
	{/*
	  var receivedMessage=new byte[] {
				0x01, 0x16, 0x7b, 0x28, 
  
				0x48, 0x32, 0x30, 0x31,   0x34, 0x30, 0x31, 0x35, 0x39, 0x38, 						//				"H201401598"
  
				0x52, 0x01,																							//0x52 0x51 
  
				0x02, 
  
				0x46, 0x52, 0x49, 0x47, 0x4f, 0x20, 0x41, 0x50, 0x4f, 0x54, 0x48, 0x45, 0x45, 0x4b, 0x20, 0x53, 	//	"FRIGO APOTHEEK S"
  
				0x00, 0x00, 0x00, 0x00, 
  
				0x00, 0x00, 0x00, 0x00, 
  
				0x01, 0x07, 
				
				0x00, 0x55, 
	  
				0x00, 0x77, 
  
				0x29, 0x7d, 0x7e, 0x04
			};
	  
	  
	  */
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] Header;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
		public byte[] SerialNumber;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[] AnswerCode;

		public int SampleInterval;
		

		public byte Unknown5;


		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] Tail;

		public bool IsValid()
		{
			return Messages.Header.SequenceEqual(Header) &&
			       Messages.AnswerGetDataInfo2Command.SequenceEqual(AnswerCode) &&
			       Messages.Tail.SequenceEqual(Tail);
		}
				
	}
}
