using TemperatuurLogger.Model;
using NUnit.Framework;

namespace TemperatuurLogger.Tests
{
    [TestFixture]
    public class DeviceServiceTests
    {
        [OneTimeSetUp] 
        public void SetUpFixture() 
        {
           TestsSetup.ResetTestDir();
        }


        [Test]
        public void Test1()
        {
            var serial = Guid.NewGuid().ToString();
            var exists = DeviceService.Instance.TemperatureLoggerIsKnown(serial);
            Assert.That(exists, Is.False);
        }

        [Test]
        public void Test2()
        {
            var serial = Guid.NewGuid().ToString();
            var name = Guid.NewGuid().ToString();
            var logger = DeviceService.Instance.CreateLogger(serial, name,DateTime.Now.Date);
            Assert.That(logger, Is.Not.Null);

            var q = DeviceService.Instance.GetLogger(serial);
            Assert.That(q, Is.Not.Null);

            DeviceService.With(() =>
            {

                Assert.That(q.SerialNumber, Is.EqualTo(serial));
                Assert.That(q.ID, Is.EqualTo(logger.ID));
            });
        }

        [Test]
        public void Test3()
        {
            var serial = Guid.NewGuid().ToString();
            var name = Guid.NewGuid().ToString();
            var logger = DeviceService.Instance.CreateLogger(serial, name, DateTime.Now.Date.AddDays(-100));

            var change = DateTime.Now.Date;
            DeviceService.Instance.RecordBatteryChange(serial, change);

            DeviceService.With(() =>
            {                
                Assert.That(change, Is.EqualTo(logger.LastBatteryRefresh));
            });
        }

        [Test]
        public void Test4()
        {
            var serial = Guid.NewGuid().ToString();
            var name = Guid.NewGuid().ToString();
            var now = DateTime.Now;
            var purchase = now.Date.AddYears(-1).AddDays(-1d);

            var logger = DeviceService.Instance.CreateLogger(serial, name, purchase);

            DeviceService.Instance.RecordBatteryChange(serial, purchase);

            var batteryChangeNeed = () => DeviceService.Instance.BatteryChangeNeeded(serial);
            Assert.That(batteryChangeNeed(), Is.True);

            DeviceService.Instance.RecordBatteryChange(serial, now);

            Assert.That(batteryChangeNeed(), Is.False);            
        }

    }
}
