using System;
using System.IO;
using static TemperatuurLogger.Model.Utils;

namespace TemperatuurLogger.ImportLogp
{
    /// <summary>
    /// Arguments encapsulation.
    /// </summary>
    public class Arguments
    {
        #pragma warning disable 1591
        public string SerialNumber { get; private set; }
        public string LogpFile { get; private set; }
        public bool Create { get; private set; }

        public bool Verbose { get; private set; }

        public Arguments Validate()
        {
            var dbfile = GetDb3FullPath();
            var dbfileExists = File.Exists(dbfile);
            var ok = true;

            if (Create == dbfileExists)
            {
                ok = false;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(($"{dbfile} {(dbfileExists ? "exists" : "does not exist")} - {(Create ? "do not" : "") } use option --create"));
            }
            if( ! File.Exists(LogpFile)) {
                ok = false;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{LogpFile} does not exist; specify an existing logp file");
            }
            if(!ok)
                Environment.Exit(1);

            return this;
        }

        /// <summary>
        /// c'tor
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="logpfile"></param>
        /// <param name="create"></param>
        /// <param name="verbose"></param>
        public Arguments(string serialNumber, string logpfile, bool create, bool verbose)
        {
            SerialNumber = serialNumber;
            LogpFile = logpfile;
            Create = create;
            Verbose = verbose;
        }
    }
}