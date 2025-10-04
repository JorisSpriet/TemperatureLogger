using System;
using System.Collections.Generic;
using System.IO;
using Xtensive.Core;
using Xtensive.Orm;
using Xtensive.Orm.Configuration;

namespace TemperatuurLogger.Model
{
    public static class DomainBuilder
    {        
        static DeviceDomain deviceDomain;
        
        static Dictionary<string, MeasurementDomain> measurementDomains = new Dictionary<string, MeasurementDomain>();

        internal static void Dispose()
        {
            if(!Utils.IsTest) throw new InvalidOperationException();
            DeviceDomain?.Domain.DisposeSafely();
            deviceDomain = null;
            measurementDomains.ForEach(md => md.Value.Domain.DisposeSafely());
            measurementDomains.Clear();
        }

        public static DeviceDomain DeviceDomain  => BuildDeviceDomain();

        public static MeasurementDomain MeasurementDomain(string year) => BuildMeasurementDomain(year);
        
        internal static DeviceDomain BuildDeviceDomain()
        {
            if(deviceDomain!=null)
                return deviceDomain;

            string dbPath = Utils.GetDeviceDbPath();

            var create = !File.Exists(dbPath);

            var domainConfiguration = new DomainConfiguration($"sqlite:///{dbPath}");

            domainConfiguration.Types.Register(typeof(TemperatureLogger));

            domainConfiguration.UpgradeMode = create ? DomainUpgradeMode.Recreate : DomainUpgradeMode.PerformSafely;

            deviceDomain = new DeviceDomain(Domain.Build(domainConfiguration));

            return deviceDomain;
        }

        internal static MeasurementDomain BuildMeasurementDomain(string year)
        {
            if(measurementDomains.ContainsKey(year))
                return measurementDomains[year];

            string dbPath = Utils.GetMeasurementDbPath(year);
               var create = !File.Exists(dbPath);
            
            var domainConfiguration = new DomainConfiguration($"sqlite:///{dbPath}");
            domainConfiguration.Types.Register(typeof(Measurement));
            domainConfiguration.Types.Register(typeof(MeasurementDownload));
            domainConfiguration.UpgradeMode = create ? DomainUpgradeMode.Recreate : DomainUpgradeMode.PerformSafely;

            var md = new MeasurementDomain(year,Domain.Build(domainConfiguration));
            measurementDomains[year] = md;            
            return md;
        }
    }
}