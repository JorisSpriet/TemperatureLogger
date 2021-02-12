using System;
using Xtensive.Orm;

namespace TemperatuurLogger.Model.Device
{
    using TemperatuurLogger.Model;

    [HierarchyRoot]
    public class Logger : BaseObject
    {
        
        [Field(Length = 20)]
        public string Name { get; set;}

        [Field(Length = 50)]
        public string Description { get; set;}

        [Field(Length=20)]
        public string SerialNumber { get; set;}

        [Field]
        public DateTime DateOfPurchase { get; set;}

        [Field]
        public DateTime LastBatteryRefresh { get; set;}
    }
}
