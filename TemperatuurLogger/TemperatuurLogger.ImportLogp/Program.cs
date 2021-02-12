using System;

namespace TemperatuurLogger.ImportLogp
{
    class Program
    {
        static void Main(string serialNumber, string logpFile, bool create=false)
        {
            var args = new Arguments(serialNumber,logpFile,create)
                .Validate();
        }
    }
}
