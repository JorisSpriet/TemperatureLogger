using System;
using Xtensive.Orm;

namespace TemperatuurLogger.Model
{
    [HierarchyRoot]
    public class MeasurementDownload : BaseObject
    {
        [Field]
        public string SerialNumber { get; private set; }

        [Field]
        public DateTime DownloadTime { get; private set; }
        
        [Field]
        public DateTime StartTime { get; private set; }

        [Field]
        public DateTime EndTime { get; private set; }

        [Field, Association(PairTo="Download",OnOwnerRemove=OnRemoveAction.Cascade, OnTargetRemove=OnRemoveAction.Deny)]
        public EntitySet<Measurement> Measurements { get; private set; }

        public MeasurementDownload(string serialNumber, DateTime downloadTime, DateTime startTime, DateTime endTime)
        {
            SerialNumber = serialNumber;
            DownloadTime = downloadTime;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}