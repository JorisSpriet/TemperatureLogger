using System;
using Xtensive.Orm;

namespace TemperatuurLogger.Model
{
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

        [Field, Association(PairTo="Download",OnOwnerRemove=OnRemoveAction.Cascade, OnTargetRemove=OnRemoveAction.Deny)]
        public EntitySet<Measurement> Measurements { get; private set; }

        public MeasurementDownload(Logger logger, DateTime downloadTime, DateTime startTime, DateTime endTime)
        {
            Logger = logger;
            DownloadTime = downloadTime;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}