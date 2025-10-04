using TemperatuurLogger.Model;
using NUnit.Framework;
using Xtensive.Orm;

namespace TemperatuurLogger.Tests
{
    [TestFixture]
    public class MeasurementModelTests
    {
        [OneTimeSetUp]
        public void SetUpFixture()
        {
            TestsSetup.ResetTestDir();
        }

        Domain domain;
        Session session;
        TransactionScope transaction;

        [SetUp]
        public void Setup()
        {
            domain = DomainBuilder.MeasurementDomain("2024").Domain;
            session = domain.OpenSession();
            session.Activate();
            transaction = session.OpenTransaction();
        }

        [TearDown]
        public void TearDown()
        {
            transaction.Dispose();
            session.Dispose();
        }

        [Test]
        public void Test1()
        {
            var serial = Guid.NewGuid().ToString();
            var start = new DateTime(2024, 12, 1, 12, 0, 0);
            var end = start.AddDays(15);            

            var d = new MeasurementDownload(serial, DateTime.Now, start, end);
            var m1 = new Measurement(d, start, 4.5M);
            var m2 = new Measurement(d, end, 4.5M);

            Assert.That(m1.Download, Is.EqualTo(d));
            Assert.That(m2.Download, Is.EqualTo(d));
            Assert.That(d.Measurements.Count, Is.EqualTo(2));
            Assert.That(d.StartTime, Is.EqualTo(start));
            Assert.That(d.EndTime,Is.EqualTo(end));
        }


    }
}
