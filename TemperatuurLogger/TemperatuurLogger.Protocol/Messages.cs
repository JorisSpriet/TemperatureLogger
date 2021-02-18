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

		public static readonly byte[] Header = {0x01, 0x16, 0x7b, 0x28,};

		public static readonly byte[] Tail = {0x29, 0x7d, 0x7e, 0x04};


		private static byte[] GetTemplate()
		{
			var result = new byte[20];
			Buffer.BlockCopy(template,0,result,0,20);
			return result;
		}


		/// <summary>
		/// PC -> logger
		/// </summary>
		public static byte[] GetSerialCommand = {0x51, 0x51};

		//
		public static byte[] GetDataInfo1Command = { 0x53, 0x29 };

		public static byte[] GetDataInfo2Command = {0x5f, 0x25};

		public static byte[] GetDataCommand = { 0x57, 0x2d };

		/// logger -> PC
		public static byte[] AnswerGetSerialCommand = { 0x52, 0x01 };

		public static byte[] AnswerGetDataInfo1Command = { 0x54, 0x01 };

		public static byte[] AnswerGetDataInfo2Command = { 0x60, 0x01 };

		public static byte[] AnswerGetDataCommand = { 0x58, 0x02 };

		public static byte[] GetSerialNumberMessage()
		{
			var result = GetTemplate();
			Buffer.BlockCopy(GetSerialCommand,0,result,14,2);
			return result;
		}

		//TODO FIXUP Message.ClearDataMessage()
		public static byte[] ClearDataMessage()
		{
			return null;
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

		public static byte[] GetDataInfo1Message(string serialNumber)
		{
			return GetCommandForSerialNumber(serialNumber, GetDataInfo1Command);
		}

		public static byte[] GetDataInfo2Message(string serialNumber)
		{
			return GetCommandForSerialNumber(serialNumber, GetDataInfo2Command);
		}

		public static byte[] GetDataMessage(string serialNumber)
		{
			return GetCommandForSerialNumber(serialNumber, GetDataCommand);
		}
	}
}
