using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TemperatuurLogger.Model;
using Xtensive.Orm;

namespace TemperatuurLogger.Model
{
    public class MeasurementService : ServiceBase
    {
        private MeasurementService() { }

        private static readonly MeasurementService instance = new MeasurementService();

        public static MeasurementService Instance => instance;

        private MeasurementDomain this[string year] => DomainBuilder.BuildMeasurementDomain(year);

        public static void With(string year, Action action)
        {
            Instance[year].With(action);
        }

        public MeasurementDownload RegisterDownloadedData(string serial, (DateTime TimeStamp, decimal Temperature)[] data)
        {
            var earliestYear = data.Min(d => d.TimeStamp).Year;
            var latestYear = data.Max(d => d.TimeStamp).Year;
            if(earliestYear!=latestYear) {
                throw new ArgumentOutOfRangeException(nameof(data), "Data spans different calendar years");
            }
            MeasurementDownload result = null;
            With(earliestYear.ToString(), () => result = InnerRegisterDownloadedData(serial, data));
            return result;
        }

        private MeasurementDownload InnerRegisterDownloadedData(string serial, (DateTime TimeStamp, decimal Temperature)[] data)
        {
            var earliest = data.Min(d => d.TimeStamp);
            var latest = data.Max(d => d.TimeStamp);
            var now = DateTime.Now;
            var md = new MeasurementDownload(serial, now, earliest, latest);
            _ = data.OrderBy(x => x.TimeStamp)
                .Select(x => new Measurement(md, x.TimeStamp, x.Temperature))
                .ToArray(); //to force enumeration and hence object creation.
            return md;
        }

        bool DownloadContainsRange(MeasurementDownload d, DateTime from, DateTime to) =>
            (d.StartTime>=from && d.StartTime<to) || (d.EndTime>from && d.EndTime<=to);
            

        public MeasurementDownload[] GetDownloads(string serial, DateTime from, DateTime to)
        {
            if(from>to) throw new ArgumentOutOfRangeException(nameof(from));
            var fromYear = from.Year;
            var toYear = to.Year;
            var downloads = new List<MeasurementDownload>();
            for(var year=fromYear;year<=toYear;year++)
            {
                With(year.ToString(), () => 
                    downloads.AddRange( Query.All<MeasurementDownload>()
                                            .Where(x => x.SerialNumber== serial)
                                            .AsEnumerable()
                                            .Where(x => DownloadContainsRange(x, from, to)).ToList())
                );
            }
            return downloads.ToArray();
        }

        public List<(DateTime downloadDate,(DateTime TimeStamp, decimal Temperature)[])> GetMeasurements(DateTime from, DateTime to)
        {
            var fromYear = from.Year.ToString();
            var result = new List<(DateTime downloadDate, (DateTime TimeStamp, decimal Temperature)[])>();
            With(fromYear, () => result = InnerGetMeasurements(from,to));
            return result;
        }

        private List<(DateTime downloadDate, (DateTime TimeStamp, decimal Temperature)[])> InnerGetMeasurements(DateTime from, DateTime to)
        {
            var downloads = Query.All<MeasurementDownload>()
                .Where(x => DownloadContainsRange(x,from,to))
                .ToList();

            var result = new List<(DateTime downloadDate, (DateTime TimeStamp, decimal Temperature)[])>();
            foreach(var download in downloads)
            {
                var d = new List<(DateTime TimeStamp, decimal Temperature)>();
                foreach(var measurement in download.Measurements)
                    d.Add((measurement.Timestamp,measurement.Temperature));
                result.Add((download.DownloadTime, d.ToArray()));
            }
            return result;
        }
    }
}
