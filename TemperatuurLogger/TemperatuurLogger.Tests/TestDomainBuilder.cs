using System.IO;
using NUnit.Framework;
using TemperatuurLogger.Model;
using Xtensive.Core;
using Xtensive.Orm;


namespace TemperatuurLogger.Tests
{
    [SetUpFixture]
    public class TestDomainBuilder
    {
        static string dbDirectory;
        static string dbFileName;

        public static Domain Domain { get; private set; }

        [OneTimeSetUp]
        public void SetUp()
        {
            var f = Path.GetTempFileName();
            File.Delete(f);
            dbDirectory = Path.GetDirectoryName(f);
            dbFileName = Path.GetFileName(f);
            DomainBuilder.BuildDomain(dbDirectory, dbFileName, true);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Domain.DisposeSafely(true);
            try
            {
                Directory.Delete(dbDirectory, true);
            }
            catch
            {
                //ignore exceptions
            }
        }

    }
}