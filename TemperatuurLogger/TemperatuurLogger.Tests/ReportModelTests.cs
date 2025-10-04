using TemperatuurLogger.Model;
using NUnit.Framework;
using Xtensive.Orm;
using TemperatuurLogger.Services;

namespace TemperatuurLogger.Tests
{
    [TestFixture]
    public class ReportModelTests
    {

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            TestsSetup.ResetTestDir();
        }

        [SetUp]
        public void Setup()
        {
            RegisterMeasurements();
        }

        [Test]
        public void Test()
        {
            var svc = new LoggerReportService();
            {
                LoggerReportData[] data = svc.GetReportDataPerDay(serial, DateTime.Parse("2024-12-01"), DateTime.Parse("2024-12-01"));
                Assert.That(data.Length, Is.EqualTo(1),()=> "Not 1 mean for 1 day");
            }
            {
                LoggerReportData[] data = svc.GetReportDataPerDay(serial, DateTime.Parse("2024-12-02"), DateTime.Parse("2024-12-03"));
                Assert.That(data.Length, Is.EqualTo(2), () => "Not 2 means for 2 days over 1 download");
            }
            {
                LoggerReportData[] data = svc.GetReportDataPerDay(serial, DateTime.Parse("2024-12-01"), DateTime.Parse("2024-12-15"));
                Assert.That(data.Length, Is.EqualTo(15), () => "Not 15 means for 15 days over 1 download");
            }
            {
                LoggerReportData[] data = svc.GetReportDataPerDay(serial, DateTime.Parse("2024-12-01"), DateTime.Parse("2024-12-16"));
                Assert.That(data.Length, Is.EqualTo(16), () => "Not 16 means for 15 days over 2 downloads");
            }
            {
                LoggerReportData[] data = svc.GetReportDataPerDay(serial, DateTime.Parse("2024-12-15"), DateTime.Parse("2024-12-16"));
                Assert.That(data.Length, Is.EqualTo(2), () => "Not 2 means for 2 days over 2 downloads");
            }
            {
                LoggerReportData[] data = svc.GetReportDataPerDay(serial, DateTime.Parse("2024-12-14"), DateTime.Parse("2024-12-17"));
                Assert.That(data.Length, Is.EqualTo(4), () => "Not 4 means for 4 days over 2 downloads");
            }

        }



        [TearDown]
        public void TearDown()
        {
        }

        string serial;


        public void RegisterMeasurements()
        {
            serial = Guid.NewGuid().ToString();
            {
                var start = new DateTime(2024, 12, 1, 0, 0, 0);
                var end = start.AddDays(15);

                var data = new List<(DateTime, decimal)>();
                for (int i = 1; i <= 15; i++)
                {
                    data.Add((start.AddDays(i - 1).AddHours(4), 3.5M));
                    data.Add((start.AddDays(i - 1).AddHours(14), 4.5M));
                }
                var d = MeasurementService.Instance.RegisterDownloadedData(serial, data.ToArray());
            }
            {
                var start = new DateTime(2024, 12, 16, 0, 0, 0);
                var end = start.AddDays(15);

                var data = new List<(DateTime, decimal)>();
                for (int i = 1; i <= 15; i++)
                {
                    data.Add((start.AddDays(i - 1).AddHours(4), 4.5M));
                    data.Add((start.AddDays(i - 1).AddHours(14), 5.5M));
                }
                var d = MeasurementService.Instance.RegisterDownloadedData(serial, data.ToArray());
            }



        }


    }
}
