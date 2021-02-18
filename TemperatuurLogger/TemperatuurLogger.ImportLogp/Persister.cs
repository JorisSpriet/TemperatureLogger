using System;
using System.Linq;
using TemperatuurLogger.ImportLogp;
using TemperatuurLogger.Model;
using Xtensive.Orm;

namespace TemperatuurLogger.Model
{
    internal class Persister : SessionBound
    {

        public void Persist(LogpFile log )
        {
            var firstItem = log.Items.OrderBy(x => x.TimeStamp()).First();
            var lastItem = log.Items.OrderByDescending(x => x.TimeStamp()).First();

            var logger = DemandLogger(log.Header.SerialNumber, log.Header.LoggerName);

            var existing = Query.All<MeasurementDownload>()
                .Any(x => x.Logger.SerialNumber == log.Header.SerialNumber && x.StartTime.Date == firstItem.TimeStamp().Date);
            if(existing) {
                throw new InvalidOperationException($"Data for logger {log.Header.SerialNumber} already imported for {firstItem.TimeStamp().Date}");
            }

            var startTime = firstItem.TimeStamp();
            var endTime = lastItem.TimeStamp();
            var downloadTime = endTime;
            var dl = new MeasurementDownload(logger, downloadTime, startTime, endTime);
            
            foreach(var li in log.Items) {
                var mi = new Measurement(dl, li.TimeStamp(), Convert.ToDecimal(li.Temperature));
            }
        }

        private Logger DemandLogger(string serialNumber, string name)
        {
            var result = Query.All<Logger>()
                .Where(x => x.SerialNumber == serialNumber).SingleOrDefault();
            return result ?? new Logger(serialNumber, name);
        }

    }
}