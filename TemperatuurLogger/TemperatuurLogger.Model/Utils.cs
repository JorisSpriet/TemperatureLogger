using System;
using System.IO;
using Xtensive.Orm.Configuration;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TemperatuurLogger.Tests")]

namespace TemperatuurLogger.Model
{
    public static class Utils
    {
        private const string db3Directory = "Temperatuurmetingen";
        private const string db3DEVICEFILE = "Temperatuurloggers.db3";
        private const string db3MEASUREMENTFILETEMPLATE = "TemperatuurMetingen-{0}.db3";
        
        static string testDir = Path.Combine(
            Environment.GetEnvironmentVariable("TMP", EnvironmentVariableTarget.User),
            Guid.NewGuid().ToString());
        internal static bool IsTest { get; set; }
        internal static string TestDir
        {
            get { return testDir; }
            set
            {
                if(!IsTest) throw new InvalidOperationException();
                DomainBuilder.Dispose();
                testDir = value;
            }
        }

        private static string commonApplicationData =>
            IsTest ? TestDir :
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        private static void EnsureDbDirectory()
        {
            var dbdir = Path.Combine(commonApplicationData, db3Directory);
            if(!Directory.Exists(dbdir))
            {
                Directory.CreateDirectory(dbdir);
            }

            var writetest = Path.Combine(dbdir,Guid.NewGuid().ToString());
            try
            {
                var now = DateTime.Now;                
                File.WriteAllText(Path.Combine(dbdir,writetest),writetest);
            } finally
            {
                File.Delete(writetest);
            }
        }

        public static string GetDeviceDbPath()
        {
            EnsureDbDirectory();
            return Path.Combine(commonApplicationData, db3Directory, db3DEVICEFILE);
        }

        public static string GetMeasurementDbPath(string year)
        {
            EnsureDbDirectory();
            var valid = int.TryParse(year, out var yearValue) &&
                yearValue > 2000 && yearValue < 2100;
            if (!valid)
                throw new ArgumentOutOfRangeException(nameof(year), $"Invalid year '{year}'");
            return Path.Combine(commonApplicationData, db3Directory,
                string.Format(db3MEASUREMENTFILETEMPLATE, year));
        }
    }
}