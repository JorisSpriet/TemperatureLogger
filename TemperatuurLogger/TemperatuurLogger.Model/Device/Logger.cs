using System;
using Xtensive.Orm;

namespace TemperatuurLogger.Model
{
    using TemperatuurLogger.Model;

    [HierarchyRoot]
    public class Logger : BaseObject
    {
        
        [Field(Length = 20)]
        public string Name { get; private set;}

        [Field(Length = 50)]
        public string Description { get; set;}

        [Field(Length=20)]
        public string SerialNumber { get; private set;}

        [Field, Association(PairTo = "Logger", OnOwnerRemove = OnRemoveAction.Cascade, OnTargetRemove = OnRemoveAction.Deny)]
        public EntitySet<MeasurementDownload> Downloads { get; private set; }

        [Field]
        public DateTime DateOfPurchase { get; set;}

        [Field]
        public DateTime LastBatteryRefresh { get; set;}

        public Logger(string serialNumber, string name)
        {
            SerialNumber = serialNumber;
            Name = name;
        }
    }
}
