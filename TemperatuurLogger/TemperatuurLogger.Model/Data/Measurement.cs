using System;
using Xtensive.Orm;

namespace TemperatuurLogger.Model
{
    [HierarchyRoot]
    public class Measurement : BaseObject
    {
        [Field]
        public MeasurementDownload Download { get; private set; }

        [Field]
        public DateTime Timestamp { get; private set; }

        [Field]
        public decimal Temperature { get; private set; }

        public Measurement(MeasurementDownload download, DateTime timestamp, decimal temperature)
        {
            Download = download;
            Timestamp = timestamp;
            Temperature = temperature;
        }
    }
}