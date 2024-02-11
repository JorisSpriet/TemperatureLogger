using System;
using System.IO;
using Xtensive.Orm;
using Xtensive.Orm.Configuration;

namespace TemperatuurLogger.Model
{
    public static class DomainBuilder
    {
        static bool built;
        static Domain domain;

        public static Domain Domain => domain;

        public static Domain BuildDomain(string dbDirectory = null, string dbFileName="Temperatuurlogger.db3", bool create = false)
        {
            if (built)
                return domain;

            var userDir = dbDirectory ?? Utils.GetUserProfilePath();
            var dataFile = Path.Combine(userDir, dbFileName);

            if(create && File.Exists(dataFile)) {
                throw new InvalidOperationException($"Asked to create, but file '{dataFile}' exists !");
            }
            
            var domainConfiguration = new DomainConfiguration($"sqlite:///{dataFile}");
            
            domainConfiguration.Types.Register(typeof(Logger).Assembly);

            domainConfiguration.UpgradeMode = create ? DomainUpgradeMode.Recreate : DomainUpgradeMode.PerformSafely;


            domain = Domain.Build(domainConfiguration);
            
            return domain;
        }

        static void UnloadDomain(Domain domain)
        {
            domain.Dispose();
        }
    }
}