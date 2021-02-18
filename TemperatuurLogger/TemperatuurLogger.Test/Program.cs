using System;
using TemperatuurLogger.Protocol;

namespace TemperatuurLogger.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // var t = typeof(AnswerGetInfoDetails1Message);
            // Console.WriteLine(sizeof(t));

            var df = new DeviceFinder();
            var d = df.FindLoggerOnPort("/dev/ttyUSB0");

            if(d==null) {
                Console.WriteLine("No device found. Terminating.");
                return;
            }
            Console.WriteLine($"Device located : {d.SerialNumber}");

            var dd = d.GetDetailsFromDevice();
            Console.WriteLine("Device details:");
            Console.WriteLine($"Serial Number   : {dd.SerialNumber}");
            Console.WriteLine($"Description     : {dd.Description}");
            Console.WriteLine($"Information     : {dd.Info}");

        }
    }
}
