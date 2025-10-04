using System;
using Xtensive.Orm;

namespace TemperatuurLogger.Model
{
    [HierarchyRoot]
    public class TemperatureLogger : BaseObject
    {
        
        [Field(Length = 20)]
        public string Name { get; private set;}

        [Field(Length = 50)]
        public string Description { get; set;}

        [Field(Length=20)]
        public string SerialNumber { get; private set;}

        [Field]
        public DateTime DateOfPurchase { get; private set;}

        [Field]
        public DateTime? LastDownloadTime {  get; set; }

        [Field]
        public DateTime? LastBatteryRefresh { get; set;}

        public TemperatureLogger(string serialNumber, string name, DateTime dateOfPurchase)
        {
            SerialNumber = serialNumber;
            Name = name;
            DateOfPurchase = dateOfPurchase;
        }
    }
}
