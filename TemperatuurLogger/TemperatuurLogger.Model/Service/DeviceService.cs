using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using TemperatuurLogger.Model;
using Xtensive.Orm;

namespace TemperatuurLogger.Model
{
    public class DeviceService : ServiceBase
    {
        private DeviceService() { }

        private static readonly DeviceService instance = new DeviceService();

        public static DeviceService Instance =>  instance;

        private DeviceDomain deviceDomain => DomainBuilder.BuildDeviceDomain();

        public bool TemperatureLoggerIsKnown(string serialNumber)
        {
            var result = false;
            deviceDomain.With(() =>
            {
                result = Query.All<TemperatureLogger>().SingleOrDefault(x => x.SerialNumber == serialNumber) !=null;
            });
            return result;
        }

        public bool BatteryChangeNeeded(string serialNumber)
        {
            var result = false;
            deviceDomain.With(() =>
            {
                var logger = Query.All<TemperatureLogger>().Single(x => x.SerialNumber == serialNumber);
                result = ( DateTime.Now-logger.LastBatteryRefresh )?.TotalDays > 365;
            });
            return result;
        }

        public TemperatureLogger GetLogger(string serialNumber)
        { 
            TemperatureLogger result = null;
            deviceDomain.With(() =>
            {
                result = Query.All<TemperatureLogger>().SingleOrDefault(x => x.SerialNumber == serialNumber);
            });
            return result;
        }

        public TemperatureLogger CreateLogger(string serialNumber, string name, DateTime dateOfPurchase)
        {
            TemperatureLogger result = null;
            deviceDomain.With(() => result = new TemperatureLogger(serialNumber, name, dateOfPurchase));
            return result;
        }

        public void RecordBatteryChange(string serialNumber, DateTime when)
        {
            deviceDomain.With(() =>
            {
                Query.All<TemperatureLogger>().Single(x => x.SerialNumber==serialNumber).LastBatteryRefresh=when;
            });
        }

        public TemperatureLogger[] GetAllLoggers()
        {
            TemperatureLogger[] result = Array.Empty<TemperatureLogger>();
            deviceDomain.With(() => result = Query.All<TemperatureLogger>().ToArray());
            return result;
        }

        public static void With(Action action)
        {
            Instance.deviceDomain.With(action);
        }
        
    }
}
