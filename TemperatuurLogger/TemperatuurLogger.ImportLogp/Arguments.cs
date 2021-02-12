using System;
using System.IO;

namespace TemperatuurLogger.ImportLogp
{
    public class Arguments
    {
        private string serialNumber;
        private string logpfile;
        private bool create;

        private string GetUserProfilePath()
        {
            var currentPlatform = Environment.OSVersion.Platform;
            switch(currentPlatform) {
                case PlatformID.Win32NT:
                return System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            case PlatformID.Unix:
                return System.Environment.GetEnvironmentVariable("HOME");
            default:
                throw new Exception($"Platform {currentPlatform} not supported.");
        }

        public Arguments Validate()
        {
            var homeDir = GetUserProfilePath();
            var target = Path.Combine(, Path.Combine("TemperatuurLogger", serialNumber));
            var targetExists = File.Exists(target);
            var ok = true;

            if (create == targetExists)
            {
                ok = false;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(($"{target} {(targetExists ? "exists" : "does not exist")} - {(create ? "do not" : "") } use option --create"));
            }
            if( ! File.Exists(logpfile)) {
                ok = false;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{logpfile} does not exist; specify an existing logp file");
            }
            if(!ok)
                Environment.Exit(1);

            return this;
        }

        public Arguments(string serialNumber, string logpfile, bool create)
        {
            this.serialNumber=serialNumber;
            this.logpfile = logpfile;
            this.create = create;
        }
    }
}