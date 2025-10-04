using TemperatuurLogger.Model;
using NUnit.Framework;

namespace TemperatuurLogger.Tests
{
    [TestFixture]
    public class DomainBuildTests
    {
        [SetUp]
        public void SetUp() 
        { 
            TestsSetup.ResetTestDir();
        }


        [Test]
        public void Test1()
        {
            var deviceDomain = DomainBuilder.BuildDeviceDomain();
        }

        [Test]
        public void Test2()
        {
            var m2024 = DomainBuilder.BuildMeasurementDomain("2024");
            Assert.That(m2024.Year,Is.EqualTo("2024"));
        }

        [Test]
        public void Test3()
        {
            var m2024 = DomainBuilder.BuildMeasurementDomain("2024");           
            var m2025 = DomainBuilder.BuildMeasurementDomain("2025");

            Assert.That(Directory.Exists(Utils.TestDir), Is.True);
            Assert.That(File.Exists(Path.Combine(Utils.TestDir, "Temperatuurmetingen\\Temperatuurmetingen-2024.db3")), Is.True);
            Assert.That(File.Exists(Path.Combine(Utils.TestDir, "Temperatuurmetingen\\Temperatuurmetingen-2025.db3")), Is.True);
        }

    }
}
