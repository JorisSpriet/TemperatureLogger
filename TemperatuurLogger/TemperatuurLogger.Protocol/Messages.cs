using System;
using System.Text;

namespace TemperatuurLogger.Protocol
{
	public static class Messages
	{
		private static readonly byte[] template =
		{
			//header <SOH><SYN>{(
			0x01, 0x16, 0x7b, 0x28,										
  
			//serial number place holder
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 		
  
			//command placeholder
			0x00, 0x00,
  
			//terminator )}~<EOT>
			0x29, 0x7d, 0x7e, 0x04
		};

		public static readonly byte[] Header = { 0x01, 0x16, 0x7b, 0x28, };

		public static readonly byte[] Tail = { 0x29, 0x7d, 0x7e, 0x04 };


		private static byte[] GetTemplate()
		{
			var result = new byte[20];
			Buffer.BlockCopy(template, 0, result, 0, 20);
			return result;
		}


		/// <summary>
		/// PC -> logger
		/// </summary>
		private static byte[] GetSerialCommand = { 0x51, 0x51 };

		//
		private static byte[] GetDataInfo1Command(bool x) { return x ? new byte[]{ 0x53,0x49} : new byte[] { 0x53, 0x29 }; }

		private static byte[] GetDataInfo2Command(bool x) { return x ? new byte[] { 0x5f, 0x45 } : new byte[] { 0x5f, 0x25 }; }

		private static byte[] GetSetClockCommand(bool x) { return x ? new byte[] { 0x5b, 0x01 } : new byte[] { 0x5b, 0x01 }; }

		private static byte[] GetDataCommand = { 0x57, 0x2d };

		private static byte[] GetClearDataCommand(bool x) { return x ? new byte[] { 0x59, 0x23 } : new byte[] { 0x59, 0x23 }; }

		/// logger -> PC
		internal static byte[] AnswerGetSerialCommand = { 0x52, 0x01 };

		internal static byte[] AnswerGetDataInfo1Command = { 0x54, 0x01 };

		internal static byte[] AnswerGetDataInfo2Command = { 0x60, 0x01 };

		private static byte[] AnswerGetDataCommand = { 0x58, 0x02 };

		public static byte[] GetSerialNumberMessage()
		{
			var result = GetTemplate();
			Buffer.BlockCopy(GetSerialCommand,0,result,14,2);
			return result;
		}

		public static byte[] GetClearDataMessage(string serialNumber, bool x)
		{
			return GetCommandForSerialNumber(serialNumber, GetClearDataCommand(x));
		}

		public static byte[] GetSetClockMessage(string serialNumber, bool x)
		{
			var command = new byte[28];
			Buffer.BlockCopy(Header, 0, command, 0, 4);
			Buffer.BlockCopy(Tail, 0, command, 25, 4);
			Buffer.BlockCopy(Encoding.ASCII.GetBytes(serialNumber), 0, command, 4, 10);
			Buffer.BlockCopy(GetSetClockCommand(x), 0, command, 14, 2);
			Buffer.BlockCopy(Utils.GetTimeBCD(), 0, command, 16, 7);
			Utils.SetBcc(command);
			return command;
		}

		private static byte[] GetCommandForSerialNumber(string serialNumber, byte[] command)
		{
			if (serialNumber.Length != 10)
				throw new Exception("Invalid serial number");
			if(command.Length!=2)
				throw new Exception("Invalid command");
			var result = GetTemplate();
			var serialNumberBytes = Encoding.ASCII.GetBytes(serialNumber);
			Buffer.BlockCopy(serialNumberBytes, 0, result, 4, 10);
			Buffer.BlockCopy(command, 0, result, 14, 2);
			return result;
		}

		public static byte[] GetDataInfo1Message(string serialNumber, bool x)
		{
			return GetCommandForSerialNumber(serialNumber, GetDataInfo1Command(x));
		}

		public static byte[] GetDataInfo2Message(string serialNumber, bool x)
		{
			return GetCommandForSerialNumber(serialNumber, GetDataInfo2Command(x));
		}

		public static byte[] GetDataMessage(string serialNumber)
		{
			return GetCommandForSerialNumber(serialNumber, GetDataCommand);
		}
	}
}
