using TemperatuurLogger.Model;
using NUnit.Framework;

namespace TemperatuurLogger.Tests
{
    [TestFixture]
    public class UtilTests
    {
        [Test]
        public void Test1()
        {
            var devicedbpath = Utils.GetDeviceDbPath();
            var devicedbdir = Path.GetDirectoryName(devicedbpath);
            Assert.That(Directory.Exists(devicedbdir));
        }

        [Test]
        public void Test2()
        {
            var measurementdbpath = Utils.GetMeasurementDbPath("2025");
            var measuredbdir = Path.GetDirectoryName(measurementdbpath);
            Assert.That(Directory.Exists(measuredbdir));
        }


        [Test]
        public void Test3()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Utils.GetMeasurementDbPath("noyear");
            });
        }
        [Test]
        public void Test4()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Utils.GetMeasurementDbPath("1");
            });
        }
    }
}
