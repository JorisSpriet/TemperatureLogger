using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace TestTemperatuurLogger
{
	class Program
	{
		static void Main(string[] args)
		{
			SerialPort p = new SerialPort("COM5");
			p.BaudRate = 38400;
			p.Open();

			//p.DataReceived += new SerialDataReceivedEventHandler(p_DataReceived);

			var connectCommand = new byte[]
			{
				0x01, 0x16, 0x7b, 0x28, 
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
				0x51, 0x51,
				0x29, 0x7d, 0x7e, 0x04
			};

			p.Write(connectCommand,0,connectCommand.Length);
			Thread.Sleep(500);
			while (p.BytesToRead > 0){
				var buffer = new byte[p.BytesToRead];
				int r = p.Read(buffer, 0, buffer.Length);
				foreach (var b in buffer){
					Console.Write(b.ToString("x2")+" ");
				}
			}
			Console.WriteLine();
			Console.WriteLine("Press ENTER TO START DOWNLOAD");
			Console.ReadLine();

			var downloadCommand = new byte[]
			{
				0x01, 0x16, 0x7b, 0x28,
				//H		2			0     1			4			0			1			5			9			8			
				0x48, 0x32, 0x30, 0x31, 0x34, 0x30, 0x31, 0x35, 0x39, 0x38, 
				0x57, 0x2d,
				0x29, 0x7d, 0x7e, 0x04
			};

			//start packet = 0x01,0x16,0x7b,0x28
			//end packet = 0x29, 0x7f, 0x7en, 0x04

			p.Write(downloadCommand,0,downloadCommand.Length);
			Thread.Sleep(500);
			var total = 0;
			var packetCount = 0;

			using (var fs = File.Create("download.logp"))
				

			while (p.BytesToRead > 0){
				packetCount++;
				var buffer = new byte[p.BytesToRead];
				int r = p.Read(buffer, 0, buffer.Length);
				total += r;
				Console.Write("Received {0} bytes ({1} - {2})           \r", r,total,packetCount);	
				fs.Write(buffer,0,r);
				Thread.Sleep(500);
			}
			Console.WriteLine();
			p.Close();


			Console.ReadLine();
		}

		static void p_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			
		}
	}
}
