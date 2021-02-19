using System;
using System.IO;
using TemperatuurLogger.Protocol;
using log4net.Config;

namespace TemperatuurLogger.Test
{
    class Program
    {
        static void ConfigureLog4Net()
        {
            var d = AppDomain.CurrentDomain.BaseDirectory;
            var cfg = Path.Combine(d, "log4net.config");
            if(File.Exists(cfg))
                XmlConfigurator.ConfigureAndWatch(new FileInfo(cfg));
        }

        static void Main(string[] args)
        {
                          
            ConfigureLog4Net();

            var df = new DeviceFinder();
            var d = df.FindLoggerOnPort(DeviceFinder.DefaultPreferredPort);

            if(d==null) {
                Console.WriteLine("No device found. Terminating.");
                return;
            }
            Console.WriteLine($"Device located : {d.SerialNumber}");

            var dd = d.GetDetailsFromDevice();
            Console.WriteLine("Device details:");
            Console.WriteLine($"Serial Number   : {dd.SerialNumber}");
            Console.WriteLine($"Description     : {dd.Description}");
            Console.WriteLine($"Model           : {dd.Model}");
            Console.WriteLine($"Log Count       : {dd.NumberOfSamples}");
            Console.WriteLine($"Sample Interval : {dd.SampleInterval}");
            Console.WriteLine($"Delay Time      : {dd.DelayTime}");
            Console.WriteLine($"Offset CH1      : {dd.OffsetCh1}");
            Console.WriteLine($"Offset CH2      : {dd.OffsetCh2}");

            Console.ReadLine();

            var s = d.GetSamplesFromDevice(null).Result;

            Console.WriteLine($"Received {s.Length} samples");

            for (int i = 0; i < 10;i++) {
                var x = s[i];
                Console.WriteLine($"{x.ID:5} {x.TimeStamp:yyyy-MM-dd HH:mm:ss} {x.Temperature}");
            }


        }
    }
}
