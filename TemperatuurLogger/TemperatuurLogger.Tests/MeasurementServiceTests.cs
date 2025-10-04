using TemperatuurLogger.Model;
using NUnit.Framework;
using Xtensive.Orm;

namespace TemperatuurLogger.Tests
{
    [TestFixture]
    public class MeasurementServicetests
    {
        [SetUp]
        public void SetUp()
        {
            TestsSetup.ResetTestDir();
            //we want a new db for each test.
            //the db path is a timestamp with 1 second resolution => by waiting 2s we are sure.
            Task.Delay(2000).Wait(); 
        }


        [Test]
        public void Test1()
        {
            var serial = Guid.NewGuid().ToString();
            var start = new DateTime(2024, 12, 1, 12, 0, 0);
            var data = new[]
            {
                (start, 4.5M),
                (start.AddSeconds(15),4.6M)
            };
            var d = MeasurementService.Instance.RegisterDownloadedData(serial, data);

            Assert.That(File.Exists(Path.Combine(Utils.TestDir, "Temperatuurmetingen\\Temperatuurmetingen-2024.db3")), Is.True);

            var domain = DomainBuilder.MeasurementDomain("2024").Domain;
            using (var s = domain.OpenSession())
            {
                s.Activate();
                using (var t = s.OpenTransaction())
                {
                    var qrymd = Query.All<MeasurementDownload>();
                    var qrym = Query.All<Measurement>();
                    Assert.That(qrymd.Single(), Is.Not.Null);
                    Assert.That(qrym.Count(), Is.EqualTo(2));
                }
            }
        }

        [Test]
        public void Test2()
        {
            var serial = Guid.NewGuid().ToString();
            var start = new DateTime(2024, 12, 31, 23, 59, 59);
            var data = new[]
            {
                (start, 4.5M),
                (start.AddMinutes(15),4.6M)
            };

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                MeasurementService.Instance.RegisterDownloadedData(serial, data));
        }

        [Test]
        public void Test3()
        {
            var serial = Guid.NewGuid().ToString();
            var start = new DateTime(2024, 12, 01, 12, 0, 0);
            var data1 = new[]
            {
                (start, 4.5M),
                (start.AddMinutes(15),4.6M)
            };
            var data2 = new[]
{
                (start.AddMinutes(30d), 4.5M),
                (start.AddMinutes(45d),4.6M)
            };


            _ = MeasurementService.Instance.RegisterDownloadedData(serial, data1);
            _ = MeasurementService.Instance.RegisterDownloadedData(serial, data2);

            var domain = DomainBuilder.MeasurementDomain("2024").Domain;
            using (var s = domain.OpenSession())
            {
                s.Activate();
                using (var t = s.OpenTransaction())
                {
                    var qrymd = Query.All<MeasurementDownload>();
                    var qrym = Query.All<Measurement>();
                    Assert.That(qrymd.Count(), Is.EqualTo(2));
                    Assert.That(qrym.Count(), Is.EqualTo(4));
                }
            }
        }


    }
}
