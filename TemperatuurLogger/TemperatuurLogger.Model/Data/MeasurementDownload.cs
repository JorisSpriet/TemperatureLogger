using System;
using Xtensive.Orm;

namespace TemperatuurLogger.Model.Data
{
    using TemperatuurLogger.Model;
    using TemperatuurLogger.Model.Device;

    [HierarchyRoot]
    public class MeasurementDownload : BaseObject
    {
        [Field]
        public Logger Logger { get; private set; }

        [Field]
        public DateTime DownloadTime { get; private set; }
        
        [Field]
        public DateTime StartTime { get; private set; }

        [Field]
        public DateTime EndTime { get; private set; }

        [Field, Association("Download")]
        public EntitySet<Measurement> Measurements { get; private set; }

        public MeasurementDownload(Logger logger)
        {
            Logger = logger;
            DownloadTime = DateTime.Now;
        }
    }
}